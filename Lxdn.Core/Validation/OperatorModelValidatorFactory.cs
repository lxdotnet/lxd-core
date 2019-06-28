
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Lxdn.Core.Validation
{
    public class OperatorModelValidatorFactory
    {
        public IEnumerable<IValidator<object>> Of(object model)
        {
            return model.GetType().GetCustomAttributes<ValidateAttribute>().Select(validator =>
                new ValidatorProxy(Activator.CreateInstance(validator.Type)));
        }
    }
}