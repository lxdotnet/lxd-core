using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions
{
    [DebuggerDisplay("{Expression}")]
    public class PropertyAccessor
    {
        private readonly Delegate setter;

        private readonly Delegate getter;

        public PropertyAccessor(Expression property, params Model[] models)
            : this(property, models.Select(model => model.AsParameter()).ToArray()) { }

        public PropertyAccessor(Expression property, params ParameterExpression[] propertyParameters)
        {
            this.Expression = property;
            var value = Expression.Parameter(property.Type, "value");

            var parameters = new List<ParameterExpression> { value };
            parameters.AddRange(propertyParameters);

            //((property as PropertyExpression).Member as System.Reflection.PropertyInfo).CanWrite

            var cantWrite = ((property as MemberExpression)?.Member as PropertyInfo)?.HasPublicSetter().Negate() ?? false;

            if (!cantWrite)
            {
                Expression assignment = Expression.Assign(property, value);
                var lambda = Expression.Lambda(assignment, parameters);
                this.setter = lambda.Compile();
            }

            this.getter = Expression.Lambda(property, propertyParameters.ToArray()).Compile();
        }

        public Expression Expression { get; private set; }

        public void SetValue(object value, params object[] modelInstances)
        {
            var callParameters = new List<object> { value };
            callParameters.AddRange(modelInstances);

            this.setter.DynamicInvoke(callParameters.ToArray());
        }

        public object GetValue(params object[] modelInstances)
        {
            return this.getter.DynamicInvoke(modelInstances);
        }

        /// <summary>
        /// Returns the default value in case of any exception during retrieval
        /// </summary>
        /// <param name="modelInstances"></param>
        /// <returns></returns>
        public object GetValueOrDefault(params object[] modelInstances)
        {
            try
            {
                return this.getter.DynamicInvoke(modelInstances);
            }
            catch (Exception)
            {
                return this.Expression.Type.DefaultValue();
            }
        }
    }
}
