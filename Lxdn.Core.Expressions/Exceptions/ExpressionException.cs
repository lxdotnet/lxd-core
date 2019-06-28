
using System;

namespace Lxd.Core.Expressions.Exceptions
{
    public abstract class ExpressionException : ApplicationException
    {
        protected ExpressionException(string message) : base(message) {}

        protected ExpressionException(string message, Exception inner) : base(message, inner) { }
    }
}
