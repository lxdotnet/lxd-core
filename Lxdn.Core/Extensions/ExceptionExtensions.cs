
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lxdn.Core.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Iterates throw the exception (considering inner exceptions, aggregate exceptions)
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static IEnumerable<Exception> Flatten(this Exception ex) =>
            TreeIterator<Exception>.New().Flatten(ex, e =>
                (e as AggregateException).IfExists(aggregate => aggregate.InnerExceptions)
                ?? e.InnerException.IfExists(inner => inner.Once())
                ?? Enumerable.Empty<Exception>());
    }
}
