
using System;

namespace Lxd.Core.Validation
{
    public class ValidatorProxy : IValidator<object>
    {
        private readonly Func<object, bool> isInvalid;

        private readonly Func<object, ValidationError> getError;

        public ValidatorProxy(object validator)
        {
            this.isInvalid = (model) => (bool)validator.GetType().GetMethod("IsInvalid").Invoke(validator, new [] { model });
            this.getError = (model) => (ValidationError)validator.GetType().GetMethod("GetError").Invoke(validator, new[] { model });
        }

        public bool IsInvalid(object model)
        {
            return this.isInvalid(model);
        }

        public ValidationError GetError(object model)
        {
            return this.getError(model);
        }
    }
}