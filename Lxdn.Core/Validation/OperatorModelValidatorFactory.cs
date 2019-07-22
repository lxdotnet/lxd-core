using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Lxdn.Core.IoC;

namespace Lxdn.Core.Validation
{
    public class OperatorModelValidatorFactory
    {
        private readonly ITypeResolver resolver;

        public OperatorModelValidatorFactory(ITypeResolver resolver)
        {
            this.resolver = resolver;
        }

        public IEnumerable<IValidator<object>> Of(object model)
        {
            return model.GetType().GetCustomAttributes<ValidateAttribute>().Select(validator =>
                new ValidatorAdapter(resolver.Resolve(validator.Type)));
        }
    }
}