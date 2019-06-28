using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lxd.Core.Extensions;

namespace Lxd.Core.Validation.Collectors
{
    internal class CollectorFactory
    {
        private readonly ValidationContext validation;

        public CollectorFactory(ValidationContext validation)
        {
            this.validation = validation;
        }

        private Type GetValidatorFromProperty(PropertyInfo property)
        {
            if (this.validation.Consider(property.PropertyType))
                return typeof(CollectorForOperatorModel);

            var enumerableOf = property.PropertyType.AsArgumentsOf(typeof(IEnumerable<>));
            if (enumerableOf.HasValue && this.validation.Consider(enumerableOf.Value.Single()))
                return typeof(CollectorForOperatorModelCollection);

            return typeof(PropertyValidator);
        }

        public CollectorBase CreateFrom(OperatorModelValidator parent, PropertyInfo property)
        {
            var collector = GetValidatorFromProperty(property);
            return (CollectorBase)Activator.CreateInstance(collector, parent, property);
        }
    }
}