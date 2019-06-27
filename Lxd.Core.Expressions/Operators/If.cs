using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lxd.Core.Expressions.Operators.Models;
using Lxd.Core.Extensions;

namespace Lxd.Core.Expressions.Operators
{
    public class If : Operator
    {
        private readonly Func<Expression> create;

        public If(IfModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var condition = logic.Operators.CreateFrom(model.Condition).As<bool>();
                var then = logic.Operators.CreateFrom(model.Then);
                var @else = model.Else.IfExists(e => logic.Operators.CreateFrom(e, then.Expression.Type));

                var cases = new List<SwitchCase> { Expression.SwitchCase(then, Expression.Constant(true)) };

                // 'else' (if specified) turns to be 'default' in the switch statement:
                return Expression.Switch(condition, @else.IfExists(e => e.Expression) ?? Expression.Default(then.Expression.Type), cases.ToArray());
                // todo: try using Expression.Condition
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
