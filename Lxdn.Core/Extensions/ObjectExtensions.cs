using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxd.Core.Basics;

namespace Lxd.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasDefaultValue<TInput>(this TInput input)
        {
            return Equals(input, default(TInput));
        }

        public static TReturn IfExists<TInput, TReturn>(this TInput input, Func<TInput, TReturn> accessor)
        {
            if (input.HasDefaultValue())
            {
                // if we are returning a Task<SomeType>, return Task of SomeType.DefaultValue to enable awaiting of it:
                if (typeof(TReturn).IsGenericType && typeof(TReturn).GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var underlyingType = typeof(TReturn).GetGenericArguments().Single();
                    return (TReturn)typeof(Task).GetMethod("FromResult")?.MakeGenericMethod(underlyingType)
                        .Invoke(null, new[] { underlyingType.DefaultValue() });
                }

                return (TReturn)typeof(TReturn).DefaultValue();
            }

            return accessor(input);
        }

        public static void IfExists<TInput>(this TInput input, Action<TInput> action)
        {
            // http://stackoverflow.com/questions/5340817/what-should-i-do-about-possible-compare-of-value-type-with-null
            if (!input.HasDefaultValue())
            {
                action(input);
            }
        }

        //public static TReturn WhenExists<TInput, TReturn>(this TInput input, Func<TReturn> ctor)
        //{
        //    return input.HasDefaultValue() ? default(TReturn) : ctor();
        //}

        public static TValue ThrowIfDefault<TValue, TException>(this TValue value, Func<TException> exception)
            where TException : Exception
        {
            return value.ThrowIf(v => v.HasDefaultValue(), v => exception());
        }

        public static TValue ThrowIfDefault<TValue>(this TValue value)
        {
            return value.ThrowIfDefault(() => new InvalidOperationException());
        }

        public static TValue ThrowIf<TValue, TException>(this TValue value, Func<TValue, bool> condition, Func<TValue, TException> exception)
            where TException : Exception
        {
            if (condition(value))
                throw exception(value);
            return value;
        }

        public static object ChangeType(this object obj, Type target)
        {
            if (obj == null)
                return target.DefaultValue();

            var source = obj.GetType();

            if (source == target)
                return obj;

            if (target.IsGenericType && typeof(Nullable<>) == target.GetGenericTypeDefinition())
            {
                var underlyingType = Nullable.GetUnderlyingType(target);
                return obj.ChangeType(underlyingType);
            }

            if (obj is IConvertible && typeof(IConvertible).IsAssignableFrom(target))
            {
                if (typeof(bool) == target && typeof(string) == source)
                {
                    if (new[] { bool.FalseString, bool.TrueString }.Any(s => string.Equals(s, (string)obj, StringComparison.InvariantCultureIgnoreCase)))
                        return ((string)obj).To<bool>();

                    return Convert.ToInt32(obj) != default(int);
                }

                if (!target.IsEnum/*only known exception*/)
                    return Convert.ChangeType(obj, target);
                
                return (typeof(string) == source) 
                    ? Enum.Parse(target, (string)obj, true) 
                    : Enum.ToObject(target, Convert.ToInt32(obj));
            }

            // otherwise try an indirect conversion via invariant string:
            IFormattable formattable = obj as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString("", CultureInfo.InvariantCulture).To(target);
            }

            return obj.ToString().To(target);
        }

        public static TResult ChangeType<TResult>(this object obj)
        {
            return (TResult)obj.ChangeType(typeof(TResult));
        }

        public static IEnumerable<TItem> Once<TItem>(this TItem item)
        {
            return Enumerable.Repeat(item, 1);
        }

        public static bool IsOneOf<TEnum>(this TEnum member, params TEnum[] values)
        {
            return values.Any(value => Equals(value, member));
        }

        public static bool NotIn<TEnum>(this TEnum member, params TEnum[] values)
        {
            return values.All(value => !Equals(value, member));
        }

        public static Dictionary<string, object> ToDictionary(this object o)
        {
            if (o == null)
                return new Dictionary<string, object>();

            return o.AsDynamic().IfHasValue(dynamic => dynamic
                    .GetMetaObject(Expression.Constant(o))
                    .GetDynamicMemberNames()
                    .ToDictionary(name => name, name => ((dynamic)o)[name], StringComparer.InvariantCultureIgnoreCase)) 
                   ??
                   o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(property => property.HasPublicGetter())
                    .ToDictionary(property => property.Name, property => property.GetValue(o));
        }

        public static MayBe<IDynamicMetaObjectProvider> AsDynamic(this object obj)
        {
            var dynamicObject = obj as IDynamicMetaObjectProvider;
            return dynamicObject != null ? new MayBe<IDynamicMetaObjectProvider>(dynamicObject) : MayBe<IDynamicMetaObjectProvider>.Nothing;
        }

        public static object Call(this object item, string method, params object[] parameters)
        {
            return item.GetType().GetMethod(method).Invoke(item, parameters);
        }

        public static IPropertyAccessor PropertyOf(this object item, string name, bool caseSensitive = false, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.GetType().GetProperties(flags)
                .SingleOrDefault(candidate => 0 == string.Compare(candidate.Name, name, !caseSensitive, CultureInfo.InvariantCulture))
                .IfExists(propery => new Property(propery))
                ?.CreateAccessor(item);
        }

        public static int HashUsing<TItem>(this TItem item, params Func<TItem, object>[] properties)
        {
            unchecked // inspired by https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            {
                return properties.Aggregate((int) 2166136261, (hash, propertyOf) =>
                    (hash * 16777619) ^ (propertyOf(item)?.GetHashCode() ?? 0));
            }
        }

        private static TInput InterpretAs<TInput>(this TInput obj, Expression<Func<TInput, DateTime>> lambda, DateTimeKind kind)
        {
            var property = (lambda.Body as MemberExpression).IfExists(member => member.Member as PropertyInfo);
            if (property == null)
                throw new ArgumentException("An expression used in the lambda must be a property expression");

            property.SetValue(obj, DateTime.SpecifyKind((DateTime)property.GetValue(obj), kind));
            return obj;
        }

        public static TInput InterpretAsUtcTime<TInput>(this TInput obj, Expression<Func<TInput, DateTime>> lambda)
        {
            return InterpretAs(obj, lambda, DateTimeKind.Utc);
        }

        public static TInput InterpretAsLocalTime<TInput>(this TInput obj, Expression<Func<TInput, DateTime>> lambda)
        {
            return InterpretAs(obj, lambda, DateTimeKind.Local);
        }
    }
}
