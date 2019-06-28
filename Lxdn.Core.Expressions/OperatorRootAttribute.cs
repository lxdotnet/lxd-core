
using System;
using System.Linq;
using System.Reflection;

using Lxd.Core.Expressions;
using Lxd.Core.Expressions.Operators.Models;

[assembly: OperatorRoot(typeof(OperatorModel), As = "Core")]

namespace Lxd.Core.Expressions
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class OperatorRootAttribute : Attribute
    {
        public OperatorRootAttribute(Type root)
        {
            this.Value = root;
        }

        public Type Value { get; private set; }

        public string As { get; set; }
    }
}
