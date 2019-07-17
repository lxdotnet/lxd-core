using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Iteration;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Dynamics
{
    public static class Extensions
    {
        public static dynamic ToDynamic(this object source)
        {
            if (source == null)
                return new CaseInsensitiveExpando();

            var sourceType = source.GetType();

            //Func<Type, bool> consider = type => type.Assembly == inputType.Assembly && !type.IsValueType;

            DynamicObject toDynamic(object from)
            {
                var fromType = from.GetType();
                var enumerableOf = fromType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerableOf != null && Consider.ForIteration(enumerableOf))
                    return (from as IEnumerable).IfExists(e => e).OfType<object>()
                        .Select(member => member.ToDynamic()).OfType<DynamicObject>()
                        .Aggregate(new CaseInsensitiveEnumerableExpando(), (list, member) => list.Add(member));

                return Consider.ForIteration(fromType) ? from.ToDynamic() : null;
            }

            if (source.AsDynamic().HasValue)
                return source;

            return sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance) 
                .Where(property => property.HasPublicGetter())
                .Select(property => new { property.Name, Value = property.GetValue(source) })
                .Where(property => property.Value != null)
                .Aggregate(new CaseInsensitiveExpando(), (expando, property) =>
                    expando.Set(property.Name, toDynamic(property.Value) ?? property.Value));
        }
    }
}
