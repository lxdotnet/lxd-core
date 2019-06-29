using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lxdn.Core.Aggregates
{
    public static class Extensions
    {
        public static PropertyAccessor<TValue> SafelyBrowse<TRoot, TValue>(this TRoot root,
            Expression<Func<TRoot, TValue>> property)
        {
            Func<Expression, IEnumerable<PropertyInfo>> propertiesOf = null; // https://stackoverflow.com/a/61206

            propertiesOf = path => (path as MemberExpression).IfExists(member =>
                propertiesOf(member.Expression).With((member.Member as PropertyInfo).ThrowIfDefault()))
                ?? Enumerable.Empty<PropertyInfo>();

            var parameter = property.Parameters.Single();
            var model = new Model(parameter.Name, parameter.Type);
            var propertyPath = new PropertyPath(model, propertiesOf(property.Body));
            return new PropertyAccessor<TValue>(propertyPath.Of(root));
        }

        public static PropertyPath SafelyBrowse(this Type type, string path)
        {
            var model = new Model(type.Name.ToLowerCamelCase(), type);
            return new PropertyPath(model, path);
        }
    }
}