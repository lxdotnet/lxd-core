
using System;
using System.Collections.Generic;

namespace Lxdn.Core.Validation.Collectors
{
    internal class CollectorProxy
    {
        private readonly Func<object, object> collect;

        public CollectorProxy(object collector)
        {
            var method = collector.GetType().GetMethod("Collect");
            this.collect = (value) => method.Invoke(collector, new[] { value });
        }

        public IEnumerable<ValidationResult> Collect(object value)
        {
            return (IEnumerable<ValidationResult>)this.collect(value);
        }
    }
}