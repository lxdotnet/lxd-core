using System;
using System.Linq;
using System.Linq.Expressions;

using Lxdn.Core.Expressions.Operators.Models;
using Aggregate = Lxdn.Core.Aggregates.Property<object>;

namespace Lxdn.Core.Expressions.Operators
{
    public class Property : Operator
    {
        private readonly PropertyModel property;

        private readonly ExecutionEngine logic;

        public Property(string path, ExecutionEngine engine) : this(new PropertyModel { Path = path }, engine) { }

        public Property(PropertyModel property, ExecutionEngine logic)
        {
            this.logic = logic;
            this.property = property;
        }

        protected internal override Expression Create()
        {
            var tokens = this.property.Path.Split('.');
            var root = logic.Models[tokens.First()];

            var aggregate = new Aggregate(root, tokens);
            return aggregate.ToExpression(root.AsParameter());
        }

        public Type Type => this.Expression.Type;
    }
}
