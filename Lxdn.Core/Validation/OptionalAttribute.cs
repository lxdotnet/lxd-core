
using System;

namespace Lxd.Core.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class OptionalAttribute : Attribute { }
}
