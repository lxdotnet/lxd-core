
using System;
using System.Linq;
using System.Linq.Expressions;
using Lxd.Core.Expressions.Exceptions;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions.Operators
{
    public abstract class LogicalOperator : Operator
    {
        private readonly Func<Expression> create;

        protected LogicalOperator(LogicalOperatorModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var operands = model.Operands
                    .Select(operand => logic.Operators.CreateFrom(operand, typeof(bool)))
                    .Select(op => op.As<bool>()).ToList();

                if (operands.Count < 2)
                {
                    var id = this.GetType().GetCustomAttributes(false).OfType<OperatorAttribute>().First().Value;
                    throw new ExpressionConfigException($"'{id}' operator must contain at least 2 predicates.");
                }

                return operands.Select(op => op.Expression)
                    .Aggregate((left, right) => Expression.MakeBinary(this.ExpressionType, left, right));
            };
        }

        protected abstract ExpressionType ExpressionType { get; }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
