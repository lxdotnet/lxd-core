
using System.Reflection;
using System.Collections.Generic;

namespace Lxd.Core.Validation.Collectors
{
    internal class CollectorForOperatorModel : CollectorBase<object>
    {
        public CollectorForOperatorModel(OperatorModelValidator parent, PropertyInfo property)
            : base(parent, property) { }

        protected override IEnumerable<ValidationResult> CollectIfNotNull(object model)
        {
            return this.Current.Validate(model);
        }
    }
}