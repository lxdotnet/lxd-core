using Lxdn.Core.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Lxdn.Core.Aggregates
{
    public class PropertyAccessor
    {
        private readonly PropertyPath path;
        private readonly object root;

        public PropertyAccessor(PropertyPath path, object root)
        {
            this.path = path;
            this.root = root;
        }

        public void SetValue(object value)
        {
            if (!path.Any())
                throw new InvalidOperationException("Can't set value of the root");

            path.Last().SetValue(new PropertyPath(path.Root, path.Without(path.Last()))
                .Of(root).EnsureExists().GetValue(), value);
        }

        public object GetValue()
        {
            return path.Aggregate(root, (current, property) =>
                current.IfExists(property.GetValue));
        }

        public TValue GetValue<TValue>()
        {
            return (TValue) GetValue();
        }

        public PropertyAccessor EnsureExists()
        {
            Func<object, PropertyInfo, object> createDefault = (current, property) =>
            {
                var @default = Activator.CreateInstance(property.PropertyType);
                property.SetValue(current, @default);
                return @default;
            };

            path.Aggregate(root, (current, property) =>
                property.GetValue(current) ?? createDefault(current, property));

            return this;
        }

        public override string ToString()
        {
            return this.path.ToString();
        }
    }

    public class PropertyAccessor<TValue>
    {
        private readonly PropertyAccessor accessor;

        public PropertyAccessor(PropertyAccessor accessor)
        {
            this.accessor = accessor;
        }

        public void SetValue(TValue value)
        {
            accessor.SetValue(value);
        }

        public TValue GetValue()
        {
            return (TValue) accessor.GetValue();
        }

        public PropertyAccessor<TValue> EnsureExists()
        {
            accessor.EnsureExists();
            return this;
        }

        public override string ToString()
        {
            return accessor.ToString();
        }
    }
}