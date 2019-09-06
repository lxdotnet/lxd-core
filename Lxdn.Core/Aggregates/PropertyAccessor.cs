
using System;
using System.Linq;
using System.Diagnostics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property,nq}")]
    internal class PropertyAccessor<TReturn> : IAccessor<TReturn>
    {
        private readonly object root;

        private readonly Property<TReturn> property;
        
        public PropertyAccessor(Property<TReturn> property, object root)
        {
            this.root = root.ThrowIfDefault();
            this.property = property.ThrowIfDefault();
        }

        public void SetValue(TReturn value)
        {
            if (!property.Any())
                throw new InvalidOperationException("Can't set value of the root");

            var lastStep = property.Last();

            object valueOf(IStep step, object current)
            {
                var accessor = step.Of(current);

                object instantiate() {
                    var @new = Activator.CreateInstance(step.Type);
                    accessor.SetValue(@new);
                    return @new;
                }

                return accessor.GetValue() ?? instantiate();
            }

            var owner = property.Without(lastStep)
                .Aggregate(root, (current, step) => valueOf(step, current));

            lastStep.Of(owner).SetValue(value);
        }

        public TReturn GetValue() => (TReturn)property
            .Aggregate(root, (current, step) => current.IfExists(owner => step.Of(owner).GetValue()));
    }
}