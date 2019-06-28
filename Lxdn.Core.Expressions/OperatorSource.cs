using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Lxdn.Core.Basics;

namespace Lxdn.Core.Expressions
{
    [DebuggerDisplay("{Assembly.GetName().Name}")]
    public class OperatorSource
    {
        public OperatorSource(Assembly assembly, IEnumerable<Model> operators)
        {
            this.Assembly = assembly;
            this.Operators = operators;
        }

        public Assembly Assembly { get; private set; }

        public IEnumerable<Model> Operators { get; private set; }
    }
}