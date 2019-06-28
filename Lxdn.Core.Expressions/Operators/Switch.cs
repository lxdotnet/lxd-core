using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    public class Switch : Operator
    {
        private readonly Func<Expression> create;

        public Switch(SwitchModel @switch, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var selector = logic.Operators.CreateProperty(@switch.Key);

                var cases = @switch.Cases.Select(c =>
                    Expression.SwitchCase(logic.Operators.CreateFrom(c.Operator),
                        c.When.SplitBy(',', ' ').Select(when => when.ToExpression(selector.Type))));

                Expression @default = @switch.Default.IfExists(d => logic.Operators.CreateFrom(d));

                if (@default == null)
                {
                    // generate a default case throwing an exception:
                    var exception = CreateException(selector.Expression);

                    @default = cases.Any()
                        ? Expression.Throw(exception, cases.First().Body.Type)
                        : Expression.Throw(exception);
                }

                var selectorExpression = selector.Expression;
                if (selector.Expression.Type.IsGenericType &&
                    selector.Expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlying = selector.Expression.Type.GetGenericArguments()[0];
                    selectorExpression = Expression.Convert(selectorExpression, underlying);
                }

                return Expression.Switch(selectorExpression, @default, cases.ToArray());
            };
        }
        
        protected internal override Expression Create()
        {
            return this.create();
        }

        private static Expression CreateException(Expression selector)
        {
            // create an expression for 'throw new Exception("Don't know wat to do with " + selector.ToString())'
            var value = Expression.Call(selector, typeof(object).GetMethod("ToString", Type.EmptyTypes));

            var concat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            var message = Expression.Call(concat, Expression.Constant("Unknown switch value: "), value);

            var ctor = typeof(Exception).GetConstructor(new[] { typeof(string) });
            Debug.Assert(ctor != null); // for resharper
            return Expression.New(ctor, message);
        }
    }
}
