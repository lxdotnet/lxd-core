using System;

namespace Lxd.Core.Basics
{
    public class PropertyAccessorException : ApplicationException
    {
        public PropertyAccessorException(string message, int position)
            : base(message)
        {
            this.Position = position;
        }

        public PropertyAccessorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public int Position { get; private set; }
    }

    public class PropertyAccessorNullReferenceException : PropertyAccessorException
    {
        public PropertyAccessorNullReferenceException(string message, int position)
            : base(message, position)
        {
        }

        public PropertyAccessorNullReferenceException(int position)
            : base("Null reference exception", position)
        {
        }
    }
}
