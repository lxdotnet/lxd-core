using System;
using System.Reflection;

namespace Lxdn.Core.Expressions.Operators
{
    public class CountOfArguments : Attribute
    {
        public CountOfArguments(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }

        public static int From(Type type)
        {
            var count = type.GetCustomAttribute<CountOfArguments>();
            if (count == null)
                throw new InvalidOperationException();

            return count.Value;
        }
    }
}