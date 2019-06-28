using System.Reflection;
using System.Collections.Generic;

namespace Lxd.Core.Validation.Collectors
{
    internal class PropertyValidator : CollectorBase<object>
    {
        public PropertyValidator(OperatorModelValidator parent, PropertyInfo property) : base(parent, property) { }

        protected override IEnumerable<ValidationResult> CollectIfNotNull(object validable)
        {
            yield break;
        }
    }
}