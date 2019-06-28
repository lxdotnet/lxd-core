using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lxdn.Core.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Given an element in a sequence, returns the next element 
        /// or throws if current was the last one
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static TSource Next<TSource>(this IEnumerable<TSource> source, TSource current)
        {
            IEnumerable<TSource> rest = source.SkipWhile(item => 
                !EqualityComparer<TSource>.Default.Equals(item, current)).Skip(1);

            TSource next = rest.FirstOrDefault();

            if (Equals(default(TSource), next))
                throw new ArgumentException("The given element is the last one in the sequence");

            return next;
        }

        public static string Agglutinate<TItem>(this IEnumerable<TItem> items, Func<TItem, string> selector, string divider = null)
        {
            return string.Join(divider ?? "", items.Select(selector).ToArray());
        }

        public static string Agglutinate<TItem>(this IEnumerable<TItem> items, string divider = null)
        {
            return items.Agglutinate(item => item.ToString(), divider);
        }

        /// <summary>
        /// Given the first sequence (A1, A2, A3, ...) and the second one (B1, B2, B3),
        /// produces a new sequence (A1, B1, A2, B2, A3, B3, ...), until both sequences exhaust
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">First sequence</param>
        /// <param name="second">Second sequence</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Interchange<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> second)
        {
            return new InterchangeIterator<TSource>(source, second);
        }

        internal class InterchangeIterator<TSource> : IEnumerable<TSource>
        {
            public InterchangeIterator(IEnumerable<TSource> first, IEnumerable<TSource> second)
            {
                this.first = first.GetEnumerator();
                this.second = second.GetEnumerator();

                //this.first.Reset();
                //this.second.Reset();
            }

            private readonly IEnumerator<TSource> first;

            private readonly IEnumerator<TSource> second;

            public IEnumerator<TSource> GetEnumerator()
            {
                while (true)
                {
                    bool firstAvailable = this.first.MoveNext();
                    bool secondAvailable = this.second.MoveNext();

                    if (!firstAvailable && !secondAvailable)
                        yield break;

                    if (firstAvailable)
                        yield return this.first.Current;

                    if (secondAvailable)
                        yield return this.second.Current;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other,
                                                                                Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                   equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(T))
                   select item;

        }

        [Obsolete]
        public static object[] AsArrayOfObjects<T>(this IEnumerable<T> items)
        {
            return items.Cast<object>().ToArray();
        }

        public static IEnumerable<TItem> With<TItem>(this IEnumerable<TItem> items, TItem item)
        {
            return items.Concat(item.Once());
        }

        public static IEnumerable<T> GetBy<T>(this IEnumerable<T> items, object filter)
        {
            return items.Where(item => filter.ToDictionary().All(property =>
            {
                var current = item.GetType().GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance, null, property.Value.GetType(), Type.EmptyTypes, null);
                if (current.PropertyType != property.Value.GetType())
                    throw new ArgumentException("Incompatible property " + property.Key);

                // todo: optimize performance of the comparer (cache the type info)
                return current.PropertyType.CreateComparer().Equals(current.GetValue(item), property.Value);
            }));
        }

        public static IEnumerable<TItem> IfEmpty<TItem>(this IEnumerable<TItem> items, Func<IEnumerable<TItem>, IEnumerable<TItem>> initializer)
        {
            return items.Any() ? items : initializer(items);
        }

        public static IEnumerable<TItem> AllOf<TItem>(this IEnumerable<TItem> empty)
        {
            return Enum.GetValues(typeof(TItem)).OfType<TItem>();
        }

        public static TItem Second<TItem>(this IEnumerable<TItem> items)
        {
            return items.Skip(1).First();
        }

        public static TItem SecondOrDefault<TItem>(this IEnumerable<TItem> items)
        {
            return items.Skip(1).FirstOrDefault();
        }

        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, params TSource[] exclude)
        {
            return source.Except(exclude);
        }

        public static IEnumerable<TSource> Without<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> condition)
        {
            return source.Without(source.Where(condition).ToArray());
        }

        /// <summary>
        /// Works like this: (2, 3, 4) Xor (3, 4, 5) -> (2, 5); equatability of TSource expected
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Xor<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
            where TSource : IEquatable<TSource>
        {
            // return first.Except(second).Concat(second.Except(first)); // Except will only work with the overridden Equals and GetHashCode for complex objects

            Func<IEnumerable<TSource>, IEnumerable<TSource>, IEnumerable<TSource>> exclude = (source, some) => 
                source.Where(x => !some.Any(x.Equals));

            return exclude(first, second).Concat(exclude(second, first));
        }

        /// <summary>
        /// Iterates all members collecting possible exceptions, throws them at the end
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void SafeForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            var exceptions = new List<Exception>(0);

            foreach (var item in items)
            {
                try { action(item); }
                catch (Exception ex) { exceptions.Add(ex); }

                if (exceptions.Count == 1)
                    throw exceptions.Single();

                if (exceptions.Count > 1)
                    throw new AggregateException(exceptions);
            }
        }
    }
}