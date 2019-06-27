
using System;

namespace Lxd.Core.Expressions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AllowedValueAttribute : Attribute
    {
        public AllowedValueAttribute(object value, string name) : this(value)
        {
            this.Name = name;
        }

        public AllowedValueAttribute(object value)
        {
            this.Value = value;
        }

        public object Value { get; private set; }

        public string Name { get; private set; }
    }
}
