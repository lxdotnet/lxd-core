
using System;
using System.Linq.Expressions;

using Lxdn.Core.Expressions.Exceptions;
using Lxdn.Core.Expressions.Extensions;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
{
    public class BinaryComparer : Operator
    {
        private readonly Func<Expression> create;

        public BinaryComparer(BinaryComparerModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var left = logic.Operators.CreateFrom(model.Left);
                var right = logic.Operators.CreateFrom(model.Right, left.Expression.Type);

                if (left.Expression.Type != right.Expression.Type)
                    throw new ExpressionConfigException("Incompatible types of binary operator");

                if (model.Operation == ExpressionType.Equal || model.Operation == ExpressionType.NotEqual)
                {
                    var equals = left.Expression.Type.GetMethod("Equals", new[] { left.Expression.Type });
                    if (equals != null && !left.Expression.Type.IsValueType) // IEquatable<>
                    {
                        var compareAsEquatable = Expression.Switch(left.Expression.ToBoolean(),
                            Expression.Constant(false), // default body, not really used
                            Expression.SwitchCase(Expression.Call(left.Expression, equals, right.Expression), Expression.Constant(true)),
                            Expression.SwitchCase(Expression.Not(right.Expression.ToBoolean()), Expression.Constant(false)));

                        switch (model.Operation)
                        {
                            case ExpressionType.Equal:
                                return compareAsEquatable;

                            case ExpressionType.NotEqual:
                                return Expression.Not(compareAsEquatable);
                        }
                    }
                }
                /*
                var comparisons = new[] { ExpressionType.GreaterThanOrEqual, ExpressionType.GreaterThan, 
                    ExpressionType.LessThanOrEqual, ExpressionType.LessThan};

                if (comparisons.Any(comparison => comparison == model.Operation))
                {
                    var compareTo = left.Expression.Type.GetMethod("CompareTo", new[] { left.Expression.Type });

                    if (compareTo != null) // IComparable
                    {
                        throw new NotImplementedException(); // should be implemented the way like above
                    }
                }
                */

                if (!model.Operation.HasValue)
                    throw new ExpressionConfigException("Operation is null");

                return Expression.MakeBinary(model.Operation.Value, left.Expression, right.Expression);
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
