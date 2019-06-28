
using System;

namespace Lxd.Core.Expressions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RestrictToAttribute : Attribute
    {
        public RestrictToAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}
