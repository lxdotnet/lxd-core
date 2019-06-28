
using System;
using Lxd.Core.Validation.Collectors;

namespace Lxd.Core.Validation
{
    public class ValidationContext// : IValidationContext
    {
        public ValidationContext(Predicate<Type> consider)
        {
            this.Consider = consider;
            this.Collectors = new CollectorFactory(this);
            this.Validators = new OperatorModelValidatorFactory();
        }

        internal CollectorFactory Collectors { get; private set; }

        internal OperatorModelValidatorFactory Validators { get; private set; }

        internal OperatorModelValidator CreateFor(OperatorPath path)
        {
            return new OperatorModelValidator(this, path); // 'new' makes it be inherently thread-safe, otherwise need to synchronize the access
        }

        internal Predicate<Type> Consider { get; private set; }

        public OperatorModelValidator Create()
        {
            return this.CreateFor(OperatorPath.Empty);
        }
    }
}
