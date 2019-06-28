using System;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Exceptions;
using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    public class Throw : Operator
    {
        private readonly Func<Expression> create;

        public Throw(ThrowModel @throw, ExecutionEngine logic)
        {
            this.create = () =>
            {
                Operator message = logic.Operators.CreateFrom(@throw.Message);

                if (message.Expression.Type != typeof(string))
                    throw new ExpressionConfigException("Exception argument must be a string");

                var exceptionType = @throw.Type.IfExists(type =>
                                        logic.Operators.Models.Sources
                                            .Select(source => source.Assembly)
                                            .SelectMany(types => types.ExportedTypes)
                                            .FirstOrDefault(candidate => candidate.FullName.Contains(type) && typeof(ExpressionRuntimeException).IsAssignableFrom(candidate)))
                                    ?? typeof(ExpressionRuntimeException);

                var ctor = exceptionType.FindCtorAccepting(typeof(string)).Value;
                var ex = Expression.New(ctor, message.Expression);

                return Expression.Throw(ex, typeof(string));
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
