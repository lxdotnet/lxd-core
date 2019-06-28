
using System;
using System.Linq.Expressions;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
{
    public class Let : Operator
    {
        private readonly Func<Expression> create;

        public Let(LetModel let, ExecutionEngine logic)
        {
            var operand = logic.Operators.CreateFrom(let.Operand);
            this.Variable = new Model(let.Variable, operand.Expression.Type);

            this.create = () => Expression.Assign(this.Variable.AsParameter(), operand.Expression);
        }

        protected internal override Expression Create()
        {
            return this.create();
        }

        public Model Variable { get; private set; }
    }
}
