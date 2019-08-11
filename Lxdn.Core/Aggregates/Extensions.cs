
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Extensions;
using Lxdn.Core.Aggregates.Models;

namespace Lxdn.Core.Aggregates
{
    public static class Extensions
    {
        public static Property<TValue> Browse<TRoot, TValue>(this TRoot any, Expression<Func<TRoot, TValue>> property)
        {
            Func<Expression, IEnumerable<IStepModel>> stepsOf = null; // https://stackoverflow.com/a/61206

            PropertyInfo propertyFrom(MemberExpression member) => (member.Member as PropertyInfo)
                .ThrowIfDefault(() => new ArgumentException($"{member} must be a property", nameof(property)));

            stepsOf = path => (path as MemberExpression).IfExists(member =>
                stepsOf(member.Expression).With(new PropertyModel { Value = propertyFrom(member).Name }))
                ?? Enumerable.Empty<IStepModel>();

            var parameter = property.Parameters.Single();
            var model = new PathModel { Root = parameter.Name, Steps = stepsOf(property.Body) };

            return Property<TValue>.Factory.CreateFrom(parameter.Type, model);
        }
    }
}