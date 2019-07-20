
using System;
using System.Reflection;
using System.Diagnostics;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property.Name,nq}")]
    public class Step
    {
        private readonly PropertyInfo property;

        internal Step(PropertyInfo property)
        {
            this.property = property;
        }

        public object GetValue(object current) => property.GetValue(current);

        public void SetValue(object current, object value) => property.SetValue(current, value);

        public Type Type => property.PropertyType;

        public object Instantiate(object owner)
        {
            var value = Activator.CreateInstance(Type);
            SetValue(owner, value);
            return value;
        }

        public override string ToString() => property.Name;
    }
}