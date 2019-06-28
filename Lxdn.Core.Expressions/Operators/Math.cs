
using System;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models.Math;

namespace Lxdn.Core.Expressions.Operators
{
    public class Math : Operator
    {
        private readonly Func<Expression> create;

        public Math(MathModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var left = logic.Operators.CreateFrom(model.Left);
                var right = logic.Operators.CreateFrom(model.Right, left.Expression.Type);

                return Expression.MakeBinary(model.Operation, left.Expression, right.Expression);
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
