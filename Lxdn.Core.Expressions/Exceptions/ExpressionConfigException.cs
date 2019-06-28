using System;

namespace Lxdn.Core.Expressions.Exceptions
{
    public class ExpressionConfigException : ExpressionException
    {
        public ExpressionConfigException(string message) : base(message) {}

        public ExpressionConfigException(string message, Exception inner) : base(message, inner) { }
    }
}