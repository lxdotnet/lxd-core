using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Iteration;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Injection
{
    public static class Extensions
    {
        private const BindingFlags injectables = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Recursive injection based on the name conventions (case-insensitive).
        /// Reasonable type conversions are supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="existing"></param>
        /// <returns></returns>
        internal static object InjectTo(this object source, object existing, CultureInfo culture)
        {
            if (source == null)
                return existing;

            var dynamic = (source as IDynamicMetaObjectProvider)
                .IfExists(provider => provider.GetMetaObject(Expression.Constant(source)));

            object valueOf(string propertyName)
            {
                return dynamic != null
                    ? dynamic
                        .GetDynamicMemberNames() // can't just cast to DynamicObject here and then .GetDynamicMemberNames because not all dynamics are DynamicObject (e.g. JObject isn't)
                        .SingleOrDefault(name => string.Equals(name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                        .IfExists(name => ((dynamic)source)[name])
                    : source.GetType()
                        .GetProperty(propertyName, injectables | BindingFlags.IgnoreCase)
                        .IfExists(property => property.GetValue(source));
            }

            Func<PropertyInfo, object> recursively = target =>
            {
                var value = valueOf(target.Name);
                var targetType = target.PropertyType;

                bool consider(Type type) => Consider.ForIteration(type) && !type.IsInterface && !type.IsAbstract;

                // first examine if a collection is about to be injected
                var enumerable = targetType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerable != null && consider(enumerable))
                {
                    var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerable));
                    var add = list.GetType().GetMethod("Add");

                    return (value as IEnumerable)?.OfType<object>() // only when the value exists and only for non-null members
                        .Select(member => member.InjectTo(enumerable)).OfType<object>()
                        .Aggregate(list, (current, member) => { add.Invoke(list, new[] { member }); return list; });
                }

                // otherwise it is an injectable object or a single property:
                return consider(targetType) ? value?.InjectTo(targetType, culture) : value?.ChangeType(targetType, culture);
            };

            return existing.Inject(recursively);
        }

        internal static object Inject(this object target, Func<PropertyInfo, object> valueOf)
        {
            return target.ThrowIfDefault().GetType()
                .GetProperties(injectables)
                .Where(property => property.HasPublicSetter())
                .Select(property => new
                {
                    Target = property,
                    Value = Guard.Function(() => valueOf(property), ex =>
                        new ArgumentException(nameof(valueOf), $"Error getting value of '{property.Name}'", ex))
                })
                .Where(property => !Equals(property.Value, null))
                .Aggregate(target, (to, injectable) => Guard.Function(() => to.SetValue(injectable.Target, injectable.Value), ex
                    => new InvalidOperationException($"Error setting value '{injectable.Value}' to property '{injectable.Target.Name}'", ex)));
        }

        public static TOutput InjectTo<TOutput>(this object input) where TOutput : class, new()
            => (TOutput)input.InjectTo(new TOutput(), CultureInfo.InvariantCulture);

        public static TOutput InjectTo<TOutput>(this object input, CultureInfo culture) where TOutput : class, new()
            => (TOutput)input.InjectTo(new TOutput(), culture);

        public static object InjectTo(this object input, Type target)
            => input.InjectTo(Activator.CreateInstance(target), CultureInfo.InvariantCulture);

        public static object InjectTo(this object input, Type target, CultureInfo culture)
            => input.InjectTo(Activator.CreateInstance(target), culture);

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source) where TTarget : class
            => (TTarget)source.InjectTo(target, CultureInfo.InvariantCulture);

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source, CultureInfo culture) where TTarget : class
            => (TTarget)source.InjectTo(target, culture);
    }
}
