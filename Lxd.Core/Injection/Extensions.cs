using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxd.Core.Extensions;

namespace Lxd.Core.Injection
{
    public static class Extensions
    {
        internal static object InjectTo(this object source, object existing)
        {
            if (source == null)
                return existing;

            var dynamic = source as IDynamicMetaObjectProvider;

            var targetType = existing.GetType();

            Func<object, PropertyInfo, object> inject = (from, to) =>
            {
                Func<Type, bool> consider = type => Consider.ForIteration(type) && !type.IsInterface && !type.IsAbstract;
                var propertyType = to.PropertyType;

                // first examine if a collection is about to be injected
                var enumerableOf = propertyType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerableOf != null && consider(enumerableOf) && !propertyType.GetCustomAttributes<TreatAsObjectAttribute>(true).Any())
                {
                    return (from as IEnumerable).IfExists(v => v).OfType<object>()
                        .Select(member => member.InjectTo(enumerableOf)).OfType<object>()
                        .Aggregate(Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerableOf)),
                            (list, member) => { list.Call("Add", member); return list; });
                }

                // otherwise it is an injectable object or a single property:
                return consider(propertyType) ? from.InjectTo(propertyType) : from.ChangeType(propertyType);
            };

            Func<string, object> valueOf = propertyName =>
            {
                Func<string, bool> matching = candidate => string.Equals(candidate, propertyName, StringComparison.InvariantCultureIgnoreCase);

                return dynamic != null
                    ? dynamic
                        .GetMetaObject(Expression.Constant(source))
                        .GetDynamicMemberNames() // can't just cast to DynamicObject here and then .GetDynamicMemberNames because not all dynamics are DynamicObject (e.g. JObject isn't)
                        .SingleOrDefault(matching)
                        .IfExists(name => ((dynamic) source)[name])
                    : source.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .SingleOrDefault(candidate => matching(candidate.Name))
                        .IfExists(property => property.GetValue(source));
            };

            return targetType.GetProperties()
                .Where(property => property.HasPublicSetter())
                .Select(property => new // get source value:
                {
                    Target = property,
                    Source = valueOf(property.Name)
                })
                //.Where(injectable => injectable.Source != null)
                .Where(injectable => !Equals(injectable.Source, null))
                .Select(injectable => new // inject source value to target value:
                {
                    injectable.Target,
                    Value = Guard.Function(() => inject(injectable.Source, injectable.Target), ex => 
                        new ArgumentException(nameof(source), $"Error injecting property '{injectable.Target.Name}'", ex))
                })
                .Aggregate(existing, (to, injectable) => // set target value 
                {
                    injectable.Target.SetValue(to, injectable.Value);
                    return to;
                });
        }

        public static TOutput InjectTo<TOutput>(this object input)
            where TOutput : class, new()
        {
            return (TOutput)input.InjectTo(new TOutput());
        }

        public static object InjectTo(this object input, Type target)
        {
            return input.InjectTo(Activator.CreateInstance(target));
        }

        public static TTarget InjectFrom<TTarget>(this TTarget target, object source)
            where TTarget : class
        {
            return (TTarget)(source ?? new object()).InjectTo(target);
        }
    }
}
