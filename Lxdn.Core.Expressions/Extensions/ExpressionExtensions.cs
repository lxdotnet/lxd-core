
using System;
using System.Linq.Expressions;

namespace Lxdn.Core.Expressions.Extensions
{
    internal static class ExpressionExtensions
    {
        public static Expression ToBoolean(this Expression expression)
        {
            if (typeof(bool) == expression.Type)
                return expression;

            if (typeof(string) == expression.Type)
            {
                var notNull = Expression.MakeBinary(ExpressionType.NotEqual, expression, Expression.Constant(null, typeof(string)));
                var notEmpty = Expression.MakeBinary(ExpressionType.NotEqual, expression, Expression.Constant("", typeof(string)));
                return Expression.MakeBinary(ExpressionType.And, notNull, notEmpty);
            }

            if (expression.Type.IsGenericType &&
                typeof(Nullable<>) == expression.Type.GetGenericTypeDefinition())
            {
                //return Expression.Property(expression, "HasValue");
                return Expression.MakeBinary(ExpressionType.NotEqual, expression, Expression.Constant(null));
            }

            Expression defValue = Expression.Default(expression.Type);
            return Expression.MakeBinary(ExpressionType.NotEqual, expression, defValue);
        }
    }
}
