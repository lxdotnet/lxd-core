using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Globalization;
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

            var dynamic = source.GetDynamicMetaObject();

            object valueOf(string propertyName)
            {
                return dynamic != null
                    ? dynamic
                        .GetDynamicMemberNames()
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
                var memberType = targetType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (memberType != null && consider(memberType))
                {
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(memberType));

                    return (value as IEnumerable)?.OfType<object>() // only when the value exists and only for non-null members
                        .Select(member => member.InjectTo(memberType)).OfType<object>()
                        .Aggregate(list, (current, member) => { current.Add(member); return current; });
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

        public static TTarget InjectTo<TTarget>(this object source) where TTarget : class, new()
            => (TTarget)source.InjectTo(new TTarget(), CultureInfo.InvariantCulture);

        public static TTarget InjectTo<TTarget>(this object source, CultureInfo culture) where TTarget : class, new()
            => (TTarget)source.InjectTo(new TTarget(), culture);

        public static object InjectTo(this object source, Type target)
            => source.InjectTo(Activator.CreateInstance(target), CultureInfo.InvariantCulture);

        public static object InjectTo(this object source, Type target, CultureInfo culture)
            => source.InjectTo(Activator.CreateInstance(target), culture);

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source) where TTarget : class
            => (TTarget)source.InjectTo(target, CultureInfo.InvariantCulture);

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source, CultureInfo culture) where TTarget : class
            => (TTarget)source.InjectTo(target, culture);
    }
}
