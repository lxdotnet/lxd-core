
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property,nq}")]
    public class PropertyAccessor<TValue>
    {
        private readonly Property<TValue> property;
        private readonly object root;

        public PropertyAccessor(Property<TValue> property, object root)
        {
            this.root = root;
            this.property = property;
        }

        public void SetValue(TValue value)
        {
            if (!property.Any())
                throw new InvalidOperationException("Can't set value of the root");

            property.Last().SetValue(new Property<object>(property.Root, property.Without(property.Last()))
                .Of(root).EnsureExists().GetValue(), value);
        }

        public TValue GetValue()
        {
            return (TValue) property.Aggregate(root, (current, property) =>
                current.IfExists(property.GetValue));
        }

        public PropertyAccessor<TValue> EnsureExists()
        {
            Func<object, PropertyInfo, object> createDefault = (current, property) =>
            {
                var @default = Activator.CreateInstance(property.PropertyType);
                property.SetValue(current, @default);
                return @default;
            };

            property.Aggregate(root, (current, property) =>
                property.GetValue(current) ?? createDefault(current, property));

            return this;
        }
    }
}