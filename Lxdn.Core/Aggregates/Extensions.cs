
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
        public static Property<TValue> Browse<TRoot, TValue>(this TRoot anyValue, Expression<Func<TRoot, TValue>> property)
        {
            Func<Expression, IEnumerable<PropertyInfo>> propertiesOf = null; // https://stackoverflow.com/a/61206

            propertiesOf = path => (path as MemberExpression).IfExists(member =>
                propertiesOf(member.Expression).With((member.Member as PropertyInfo).ThrowIfDefault()))
                ?? Enumerable.Empty<PropertyInfo>();

            var parameter = property.Parameters.Single();
            var model = new Model(parameter.Name, parameter.Type);
            return new Property<TValue>(model, propertiesOf(property.Body));
        }
    }
}