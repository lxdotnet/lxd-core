
using System;

namespace Lxd.Core.Expressions
{
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(object value)
        {
            this.Value = value;
        }

        public object Value { get; private set; }
    }
}
