using System;

namespace Lxd.Core.Injection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectUsingAttribute : Attribute
    {
        public Type Factory { get; }

        public InjectUsingAttribute(Type factory)
        {
            Factory = factory;
        }
    }
}