using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Lxd.Core.Extensions;

namespace Lxd.Core.Validation.Collectors
{
    internal class CollectorForOperatorModelCollection : CollectorBase<IEnumerable<object>>
    {
        public CollectorForOperatorModelCollection(OperatorModelValidator parent, PropertyInfo property)
            : base(parent, property) { }

        protected override IEnumerable<ValidationResult> CollectIfNotNull(IEnumerable<object> models)
        {
            return Enumerable.Range(0, models.Count())
                .Zip(models, (index, model) => new { Path = this.Current.Path.TakeIndex(index), Model = model })
                .SelectMany(nested => nested.Model == null
                    ? MissingValue.For(nested.Path).Once()
                    : this.Current.Validation.CreateFor(nested.Path).Validate(nested.Model));
        }
    }
}