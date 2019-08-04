
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("{property.Name,nq}")]
    public class Step : IStep
    {
        private readonly PropertyInfo property;

        private object EnsureExists(object owner) =>
            owner.ThrowIfDefault(() => new NullReferenceException($"The value of '{property.Name}' is null"));

        internal Step(PropertyInfo property)
        {
            this.property = property;
        }

        public object GetValue(object current) => Guard.Function(() => 
            property.GetValue(EnsureExists(current)), 
            ex => new InvalidOperationException($"Cannot get value of '{property.Name}' in {current}", ex));

        public void SetValue(object current, object value) => Guard.Action(() =>
            property.SetValue(EnsureExists(current), value),
            ex => new InvalidOperationException($"Cannot set value of '{property.Name}' in {current}", ex));

        public Type Type => property.PropertyType;

        public object InstantiateIn(object owner)
        {
            var value = Guard.Function(() => Activator.CreateInstance(Type),
                ex => new InvalidOperationException($"Failed to instantiate '{property.Name}'"));

            SetValue(owner, value);
            return value;
        }

        public Expression ToExpression(Expression current) => Expression.Property(current, property);

        public override string ToString() => $".{property.Name}";
    }
}