
using System;
using System.Linq;
using System.Diagnostics;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property,nq}")]
    public class PropertyAccessor<TValue>
    {
        private readonly object root;

        private readonly Property<TValue> property;
        
        public PropertyAccessor(Property<TValue> property, object root)
        {
            this.root = root;
            this.property = property;
        }

        public void SetValue(TValue value)
        {
            if (!property.Any())
                throw new InvalidOperationException("Can't set value of the root");

            var lastStep = property.Last();

            var owner = property.Without(lastStep)
                .Aggregate(root, (current, step) => step.GetValue(current) ?? step.Instantiate(current));

            lastStep.SetValue(owner, value);
        }

        public TValue GetValue() => (TValue)property
            .Aggregate(root, (current, step) => current.IfExists(step.GetValue));
    }
}