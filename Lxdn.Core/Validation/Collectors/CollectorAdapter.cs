
using System;
using System.Collections.Generic;

namespace Lxdn.Core.Validation.Collectors
{
    internal class CollectorAdapter
    {
        private readonly Func<object, IEnumerable<ValidationResult>> collect;

        public CollectorAdapter(object collector)
        {
            var method = collector.GetType().GetMethod("Collect");
            this.collect = value => (IEnumerable<ValidationResult>)method.Invoke(collector, new[] { value });
        }

        public IEnumerable<ValidationResult> Collect(object value) => collect(value);
    }
}