using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lxd.Core.Expressions.Exceptions;
using Lxd.Core.Expressions.Operators.Models.Output;

namespace Lxd.Core.Expressions.Operators
{
    public class StringFormat : Operator
    {
        private readonly Func<Expression> create;

        public StringFormat(StringFormatModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                List<Expression> parameters = model.Arguments
                    .Select(argument => logic.Operators.CreateFrom(argument))
                    .Select(op => op.Expression).ToList();

                // use the following signature: String.Format(string format, params object[] args)
                // todo: we must consider the desired culture here, which is not necessarily the current thread's culture

                Expression voidParameter = parameters.FirstOrDefault(p => typeof(void) == p.Type);
                if (voidParameter != null)
                    throw new ExpressionConfigException("Void parameters are not allowed: " + voidParameter);

                // make the arguments be of the same type 'object', cast if necessary
                parameters = parameters.Select(p => typeof(object) == p.Type
                    ? p : Expression.Convert(p, typeof(object))).ToList();

                Expression array = Expression.NewArrayInit(typeof(object), parameters);
                return Expression.Call(Format, new[] { logic.Operators.CreateFrom(model.Format).Expression, array });
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }

        static StringFormat()
        {
            Format = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object[]) });
        }

        private static readonly MethodInfo Format;
    }
}
