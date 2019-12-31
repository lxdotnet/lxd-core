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
        public static TDynamicObject ToDynamic<TDynamicObject>(this object source)
            where TDynamicObject : DynamicObject, new()
        {
            if (source == null)
                return new TDynamicObject();

            var sourceType = source.GetType();

            //Func<Type, bool> consider = type => type.Assembly == inputType.Assembly && !type.IsValueType;

            DynamicObject toDynamic(object from)
            {
                var fromType = from.GetType();
                var memberType = fromType.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (memberType != null && Consider.ForIteration(memberType))
                    return (from as IEnumerable)
                        .IfExists().OfType<object>()
                        .Select(member => member.ToDynamic<TDynamicObject>())
                        .OfType<TDynamicObject>()
                        .Aggregate(new CaseInsensitiveEnumerableExpando(), (list, member) => list.Push(member));

                return Consider.ForIteration(fromType) ? from.ToDynamic<TDynamicObject>() : null;
            }

            //if (source.AsDynamic().HasValue)
            //    return (TDynamicObject)source; // may crash 

            return sourceType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.HasPublicGetter())
                .Select(property => new { property.Name, Value = property.GetValue(source) })
                .Where(property => property.Value != null)
                .Aggregate(new TDynamicObject(), (impl, property) =>
                {
                    var dynamic = impl as dynamic;
                    dynamic[property.Name] = toDynamic(property.Value) ?? property.Value;
                    return impl;
                });
        }

        public static dynamic ToDynamic(this object source) =>
            source.GetDynamicMetaObject().IfExists(dynamic => dynamic.Value)
            ?? source.ToDynamic<CaseInsensitiveExpando>();
    }
}
