
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property.Name,nq}")]
    public class Step
    {
        private readonly PropertyInfo property;

        internal Step(PropertyInfo property)
        {
            this.property = property;
        }

        public object GetValue(object current) => Guard.Function(() => property.GetValue(current
            .ThrowIfDefault(() => new NullReferenceException($"The value of '{property.Name}' is null"))), 
             ex => new InvalidOperationException($"Cannot get value of '{property.Name}'", ex));

        public void SetValue(object current, object value) => property.SetValue(current, value);

        public Type Type => property.PropertyType;

        public object Instantiate(object owner)
        {
            var value = Activator.CreateInstance(Type);
            SetValue(owner, value);
            return value;
        }

        public Expression ToExpression(Expression of) => Expression.Property(of, property);

        public override string ToString() => property.Name;
    }
}