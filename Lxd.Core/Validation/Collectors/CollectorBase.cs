using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Lxd.Core.Extensions;
using Lxd.Core.Validation.Extensions;
using System;

namespace Lxd.Core.Validation.Collectors
{
    internal abstract class CollectorBase
    {
        public CollectorProxy AsProxy()
        {
            return new CollectorProxy(this);
        }

        protected static readonly ValidationError MissingValue = new ValidationError("MissingValue", "Missing property");
    }

    internal abstract class CollectorBase<TValidable> : CollectorBase where TValidable : class
    {
        protected CollectorBase(OperatorModelValidator parent, PropertyInfo property)
        {
            this.Property = property;
            this.Current = parent.StepInto(property);
        }

        public IEnumerable<ValidationResult> Collect(TValidable validable)
        {
            var canBeNull = this.Property.PropertyType.AsArgumentsOf(typeof(Nullable<>)).HasValue || !this.Property.PropertyType.IsValueType;
            if (canBeNull && validable == null)
            {
                return this.Property.IsOptional()
                    ? Enumerable.Empty<ValidationResult>()
                    : MissingValue.For(this.Current.Path).Once();
            }

            return this.CollectIfNotNull(validable);
        }

        protected abstract IEnumerable<ValidationResult> CollectIfNotNull(TValidable validable);

        protected PropertyInfo Property { get; private set; }

        protected OperatorModelValidator Current { get; private set; }
    }
}