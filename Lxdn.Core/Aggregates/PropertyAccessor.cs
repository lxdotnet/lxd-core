
using System;
using System.Linq;
using System.Diagnostics;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property,nq}")]
    public class PropertyAccessor<TReturn>
    {
        private readonly object root;

        private readonly Property<TReturn> property;
        
        public PropertyAccessor(Property<TReturn> property, object root)
        {
            this.root = root.ThrowIfDefault();
            this.property = property.ThrowIfDefault();
        }

        public void Set(TReturn value)
        {
            if (!property.Any())
                throw new InvalidOperationException("Can't set value of the root");

            var lastStep = property.Last();

            var owner = property.Without(lastStep)
                .Aggregate(root, (current, step) => step.GetValue(current) ?? step.InstantiateIn(current));

            lastStep.SetValue(owner, value);
        }

        public TReturn Get() => (TReturn)property
            .Aggregate(root, (current, step) => current.IfExists(step.GetValue));
    }
}