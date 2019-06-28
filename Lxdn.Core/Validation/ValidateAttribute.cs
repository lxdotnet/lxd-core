using System;

namespace Lxdn.Core.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ValidateAttribute : Attribute
    {
        public ValidateAttribute(Type validator)
        {
            this.Type = validator;
        }

        public Type Type { get; private set; }
    }
}