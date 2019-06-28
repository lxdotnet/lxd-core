
using System;
using System.Collections.Generic;

namespace Lxdn.Core.Basics
{
    public class MayBe<TValue>
        where TValue : class
    {
        private MayBe() {}

        static MayBe()
        {
            Nothing = new MayBe<TValue>();
        }

        public static MayBe<TValue> Nothing { get; private set; }

        public MayBe(TValue value)
        {
            this.Value = value;
        }

        public MayBe<TValue> ThrowIfHasNoValue<TException>(Func<TException> exception)
            where TException : Exception
        {
            if (!this.HasValue)
                throw exception();

            return this;
        }

        public TResult IfHasValue<TResult>(Func<TValue, TResult> doSomething)
        {
            if (!this.HasValue)
                return default(TResult);

            return doSomething(this.Value);
        }

        public bool HasValue
        {
            get { return this.Value != null; }
        }

        public TValue Value { get; private set; }

        public static MayBe<TValue> FromValue(TValue value)
        {
            return !EqualityComparer<TValue>.Default.Equals(default(TValue), value) ? new MayBe<TValue>(value) : Nothing;
        }
    }
}
