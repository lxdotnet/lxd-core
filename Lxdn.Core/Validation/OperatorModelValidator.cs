
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Validation
{
    public class OperatorModelValidator
    {
        internal OperatorModelValidator(ValidationContext validation, OperatorPath path)
        {
            this.Path = path;
            this.Validation = validation;
        }

        internal ValidationContext Validation { get; private set; }

        internal OperatorPath Path { get; private set; }

        public IEnumerable<ValidationResult> Validate(object model)
        {
            var local = this.Validation.Validators.Of(model)
                .Where(validator => validator.IsInvalid(model))
                .Select(validator => validator.GetError(model).For(this.Path));

            var nested = model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.HasPublicGetter() && property.HasPublicSetter())
                .Select(property => new { Value = property.GetValue(model), Collector = this.Validation.Collectors.CreateFrom(this, property) })
                .SelectMany(property => property.Collector.CreateAdapter().Collect(property.Value));

            return local.Union(nested);
        }

        internal OperatorModelValidator StepInto(PropertyInfo property)
        {
            var step = new OperatorStep(property);
            return new OperatorModelValidator(this.Validation, this.Path.GrowBy(step));
        }
    }
}