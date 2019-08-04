using System;
using System.Linq.Expressions;

using Lxdn.Core.Aggregates.Models;
using Aggregate = Lxdn.Core.Aggregates.Property<object>;

namespace Lxdn.Core.Expressions.Operators
{
    public class Property : Operator
    {
        private readonly Models.PropertyModel property;

        private readonly ExecutionEngine logic;

        public Property(Models.PropertyModel property, ExecutionEngine logic)
        {
            this.logic = logic;
            this.property = property;
        }

        protected internal override Expression Create()
        {
            var path = PathModel.Parse(property.Path);
            var root = logic.Models[path.Root];

            var aggregate = Aggregate.Create.From(root, path.Steps);
            return aggregate.ToExpression(root.AsParameter());
        }

        public Type Type => this.Expression.Type;
    }
}
