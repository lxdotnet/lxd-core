using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Iteration;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Injection
{
    public static class Extensions
    {
        private static readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        /// <summary>
        /// Recursive injection based on the name conventions.
        /// reasonable type conversions are supported.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="existing"></param>
        /// <returns></returns>
        internal static object InjectTo(this object source, object existing)
        {
            if (source == null)
                return existing;

            var dynamic = source as IDynamicMetaObjectProvider;

            object valueOf(string propertyName)
            {
                return dynamic != null
                    ? dynamic
                        .GetMetaObject(Expression.Constant(source))
                        .GetDynamicMemberNames() // can't just cast to DynamicObject here and then .GetDynamicMemberNames because not all dynamics are DynamicObject (e.g. JObject isn't)
                        .SingleOrDefault(name => string.Equals(name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                        .IfExists(name => ((dynamic)source)[name])
                    : source.GetType()
                        .GetProperty(propertyName, flags)
                        .IfExists(property => property.GetValue(source));
            }

            Func<object, PropertyInfo, object> recursively = (from, to) =>
            {
                bool consider(Type type) => Consider.ForIteration(type) && !type.IsInterface && !type.IsAbstract;
                var propertyType = to.PropertyType;

                // first examine if a collection is about to be injected
                var enumerable = propertyType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerable != null && consider(enumerable))
                {
                    return (from as IEnumerable)?.OfType<object>() // only when 'from' exists and only for non-null members
                        .Select(member => member.InjectTo(enumerable)).OfType<object>()
                        .Aggregate(Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerable)),
                            (list, member) => { list.Call("Add", member); return list; });
                }

                // otherwise it is an injectable object or a single property:
                return consider(propertyType) ? from?.InjectTo(propertyType) : from.ChangeType(propertyType);
            };

            return existing.Populate(property => recursively(valueOf(property.Name), property));
        }

        internal static object Populate(this object target, Func<PropertyInfo, object> valueOf)
        {
            return target.ThrowIfDefault().GetType()
                .GetProperties(flags)
                .Where(property => property.HasPublicSetter())
                .Select(property => new
                {
                    Target = property,
                    Value = Guard.Function(() => valueOf(property), ex =>
                        new ArgumentException(nameof(valueOf), $"Error getting value of '{property.Name}'", ex))
                })
                .Aggregate(target, (to, injectable) => Guard.Function(() => to.SetValue(injectable.Target, injectable.Value), ex
                    => new InvalidOperationException($"Error setting value '{injectable.Value}' to property '{injectable.Target.Name}'", ex)));
        }

        public static TOutput InjectTo<TOutput>(this object input) where TOutput : class, new()
            => (TOutput)input.InjectTo(new TOutput());

        public static object InjectTo(this object input, Type target)
            => input.InjectTo(Activator.CreateInstance(target));

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source) where TTarget : class
            => (TTarget)source.InjectTo(target);
    }
}
