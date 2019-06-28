
using System;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models.Linq;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    [CountOfArguments(2)]
    public class LinqLambda : LinqBase<LinqLambdaModel>
    {
        private readonly Func<Expression> create;

        public LinqLambda(LinqLambdaModel lambda, ExecutionEngine logic) : base(lambda, logic)
        {
            this.create = () =>
            {
                if (lambda.Body != null)
                {
                    var parameters = lambda.Parameters.SplitBy(' ', ',').ToList();

                    using (var capture = logic.Models.CreateClosureVariable(parameters.First(), this.ArgumentType)) // todo: consider further parameters if any
                    {
                        Type lambdaReturnType = this.Method.GetParameters().Last().ParameterType.GenericTypeArguments[1];
                        Operator body = logic.Operators.CreateFrom(lambda.Body, lambdaReturnType);

                        LambdaExpression lambdaExpression = Expression.Lambda(body.Expression, capture.AsParameter());
                        return Expression.Call(this.Method, this.Collection.Expression, lambdaExpression);
                    }
                }

                var optimistic = Expression.Lambda(Expression.Constant(true), Expression.Parameter(this.ArgumentType, "m"));
                return Expression.Call(this.Method, this.Collection.Expression, optimistic);
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
