using System;
using System.Diagnostics;

namespace Lxdn.Core.Expressions
{
    [DebuggerDisplay("{Value}")]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class OperatorAttribute : Attribute
    {
        public OperatorAttribute(string id)
        {
            this.Value = id;
            this.Name = id;
        }

        public OperatorAttribute(object unique) : this(unique.ToString()) {}

        public string Value { get; private set; }

        public bool Hidden { get; set; } // not to be published in the UI

        public bool Important { get; set; }

        public string Name { get; set; }
    }
}
