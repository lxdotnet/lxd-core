using System;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
{
    public class Not : Operator
    {
        private readonly Func<Expression> create;

        public Not(NotModel not, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var operand = logic.Operators.CreateFrom(not.Operand).As<bool>();
                return Expression.MakeUnary(ExpressionType.Not, operand.Expression, typeof(bool));
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
