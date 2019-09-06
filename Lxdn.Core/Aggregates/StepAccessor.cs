
using System;
using System.Reflection;

namespace Lxdn.Core.Aggregates
{
    internal struct StepAccessor : IAccessor // using structs for cheap allocation/deallocation on the stack
    {
        private readonly object owner;

        private readonly PropertyInfo property;

        public StepAccessor(PropertyInfo property, object owner)
        {
            this.property = property;
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public object GetValue()
        {
            var that = this;
            return Guard.Function(() => that.property.GetValue(that.owner),
                ex => new InvalidOperationException($"Cannot get value of '{that.property.Name}' in {that.owner}", ex));
        }

        public void SetValue(object value)
        {
            var that = this;
            Guard.Action(() => that.property.SetValue(that.owner, value),
                ex => new InvalidOperationException($"Cannot set value of '{that.property.Name}' in {that.owner}", ex));
        }
    }
}