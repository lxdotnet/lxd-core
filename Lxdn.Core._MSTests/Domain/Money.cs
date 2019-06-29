
using System;
using System.Diagnostics;

namespace Lxdn.Core._MSTests.Domain
{
    [DebuggerDisplay("{Amount}{Currency,nq}")]
    public class Money : IEquatable<Money>
    {
        public Money(decimal amount, string currency)
        {
            this.Amount = amount;
            this.Currency = currency;
        }

        public decimal Amount { get; private set; }

        public string Currency { get; private set; }

        public bool Equals(Money other)
        {
            return string.Equals(this.Currency, other.Currency) && this.Amount == other.Amount;
        }
    }
}
