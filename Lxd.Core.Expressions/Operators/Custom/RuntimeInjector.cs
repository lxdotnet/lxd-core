
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxd.Core.Basics;
using Lxd.Core.Expressions.Exceptions;

namespace Lxd.Core.Expressions.Operators.Custom
{
    internal class RuntimeInjector
    {
        private readonly IEnumerable<Model> models;

        public RuntimeInjector(IEnumerable<Model> models)
        {
            this.models = models;
        }

        public IEnumerable<Expression> Inject(MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                if (typeof(IRuntimeContext).IsAssignableFrom(parameter.ParameterType))
                {
                    yield return Expression.New(typeof(RuntimeContext).GetConstructors().Single(),
                        Expression.NewArrayInit(typeof(object), this.models.Select(model => model.AsParameter())));
                }
                else
                {
                    var model = this.models.FirstOrDefault(candidate =>
                        candidate.Type == parameter.ParameterType && candidate.Id == parameter.Name);
                    if (model != null)
                        yield return model.AsParameter();
                    else
                        throw new ExpressionConfigException("Cound not inject " + parameter.ParameterType.Name + " " + parameter.Name);
                }
            }
        }
    }
}
