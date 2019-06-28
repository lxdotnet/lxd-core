
using System;
using System.Collections.Generic;

namespace Lxdn.Core.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Iterates throw the exception (considering inner exceptions, aggregate exceptions)
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static IEnumerable<Exception> Flatten(this Exception ex)
        {
            var iterator = new TreeIterator<Exception>();

            return iterator.Flatten(ex, e =>
            {
                var aggregate = e as AggregateException;
                return aggregate != null
                    ? aggregate.InnerExceptions
                    : e.InnerException.IfExists(inner => inner.Once());
            });
        }
    }
}
