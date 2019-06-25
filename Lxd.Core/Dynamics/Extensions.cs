using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Lxd.Core.Basics;
using Lxd.Core.Extensions;

namespace Lxd.Core.Dynamics
{
    public static class Extensions
    {
        public static dynamic ToDynamic(this object source)
        {
            if (source == null)
                return new CaseInsensitiveExpando();

            var sourceType = source.GetType();

            //Func<Type, bool> consider = type => type.Assembly == inputType.Assembly && !type.IsValueType;

            Func<object, DynamicObject> toDynamic = from => // 'from' is expected to be non null
            {
                var fromType = from.GetType();
                var enumerableOf = fromType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerableOf != null && Consider.ForIteration(enumerableOf))
                    return (from as IEnumerable).IfExists(e => e).OfType<object>()
                        .Select(member => member.ToDynamic()).OfType<DynamicObject>()
                        .Aggregate(new CaseInsensitiveEnumerableExpando(), (list, member) => list.Add(member));

                return Consider.ForIteration(fromType) ? from.ToDynamic() : null;
            };

            if (source.AsDynamic().HasValue)
                return source;

            // reason: there are classes (like AspNetDictionary) which are dictionaries but all methods/properties are private

            //var dictionary = sourceType.AsArgumentsOf(typeof(IDictionary<,>));
            //if (dictionary.HasValue && dictionary.Value.First() == typeof(string))
            //{
            //    var indexer = sourceType.GetIndexer(typeof(string));
            //    var considerAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            //    return (source.PropertyOf("Keys", false, considerAll).Value as IEnumerable).OfType<string>()
            //        .Aggregate(new CaseInsensitiveExpando(), (expando, key) =>
            //            expando.Set(key, indexer.GetValue(source, new object[] { key })));
            //}

            return sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance) 
                .Where(property => property.HasPublicGetter())
                .Select(property => new { property.Name, Value = property.GetValue(source) })
                .Where(property => property.Value != null)
                .Aggregate(new CaseInsensitiveExpando(), (expando, property) =>
                    expando.Set(property.Name, toDynamic(property.Value) ?? property.Value));
        }
    }
}
