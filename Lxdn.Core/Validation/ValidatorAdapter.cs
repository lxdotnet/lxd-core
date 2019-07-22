
using System;

namespace Lxdn.Core.Validation
{
    public class ValidatorAdapter : IValidator<object>
    {
        private readonly Func<object, bool> isInvalid;

        private readonly Func<object, ValidationError> getError;

        public ValidatorAdapter(object validator)
        {
            this.isInvalid = (model) => (bool)validator.GetType().GetMethod("IsInvalid").Invoke(validator, new [] { model });
            this.getError = (model) => (ValidationError)validator.GetType().GetMethod("GetError").Invoke(validator, new[] { model });
        }

        public bool IsInvalid(object model) => isInvalid(model);

        public ValidationError GetError(object model) => getError(model);
    }
}