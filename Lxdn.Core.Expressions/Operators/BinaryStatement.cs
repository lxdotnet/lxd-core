using System;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models.Domain;

namespace Lxdn.Core.Expressions.Operators
{
    public class BinaryStatement : Operator
    {
        public object Verb { get; private set; }

        public Operator Subject { get; private set; }

        public Operator Object { get; private set; }

        private readonly Func<Expression> create;

        public BinaryStatement(BinaryStatementModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                Subject = logic.Operators.CreateFrom(model.Subject);
                Object = logic.Operators.CreateFrom(model.Object);
                Verb = logic.Operators.Verbs.Create(model.Verb, this);

                var instances = Expression.NewArrayInit(typeof(object), logic.Models.Select(m => m.AsParameter()));
                return Expression.Call(Expression.Constant(this.Verb), this.Verb.GetType().GetMethod("Apply"), instances);
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}