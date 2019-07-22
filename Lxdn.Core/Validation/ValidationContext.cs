
using System;
using Lxdn.Core.IoC;
using Lxdn.Core.Validation.Collectors;

namespace Lxdn.Core.Validation
{
    public class ValidationContext
    {
        public ValidationContext(ITypeResolver resolve, Predicate<Type> consider)
        {
            //this.Resolve = resolve;
            this.Consider = consider;            
            this.Collectors = new CollectorFactory(this);
            this.Validators = new OperatorModelValidatorFactory(resolve);
        }

        //internal ITypeResolver Resolve { get; }

        internal CollectorFactory Collectors { get; }

        internal OperatorModelValidatorFactory Validators { get; }

        internal OperatorModelValidator CreateFor(OperatorPath path)
        {
            return new OperatorModelValidator(this, path); // 'new' makes it be inherently thread-safe, otherwise need to synchronize the access
        }

        internal Predicate<Type> Consider { get; private set; }

        public OperatorModelValidator Create() => this.CreateFor(OperatorPath.Empty);
    }
}
