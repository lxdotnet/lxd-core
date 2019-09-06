
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property.Name,nq}")]
    internal class Step : IStep
    {
        private readonly PropertyInfo property;

        internal Step(PropertyInfo property)
        {
            this.property = property;
        }

        public Type Type => property.PropertyType;

        public IAccessor Of(object owner) => new StepAccessor(property, owner);

        public Expression ToExpression(Expression current) => Expression.Property(current, property);

        public override string ToString() => $".{property.Name}";
    }
}