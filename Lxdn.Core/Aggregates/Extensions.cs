
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public static class Extensions
    {
        public static Property<TValue> Browse<TRoot, TValue>(this TRoot any, Expression<Func<TRoot, TValue>> property)
        {
            Func<Expression, IEnumerable<Step>> stepsOf = null; // https://stackoverflow.com/a/61206

            PropertyInfo propertyFrom(MemberExpression member) => (member.Member as PropertyInfo)
                .ThrowIfDefault(() => new ArgumentException($"{member} must be a property", nameof(property)));

            stepsOf = path => (path as MemberExpression).IfExists(member =>
                stepsOf(member.Expression).With(new Step(propertyFrom(member))))
                ?? Enumerable.Empty<Step>();

            var parameter = property.Parameters.Single();
            var model = new Model(parameter.Name, parameter.Type);

            return new Property<TValue>(model, stepsOf(property.Body));
        }
    }
}