
using System;
using System.Linq.Expressions;
using Lxd.Core.Basics;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions.Operators
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
