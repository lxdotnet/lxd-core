
using System;
using System.Diagnostics;

namespace Lxdn.Core._MSTests.Domain
{
    [DebuggerDisplay("{Amount}{Currency,nq}")]
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public bool Equals(Money other)
        {
            return string.Equals(this.Currency, other.Currency) && this.Amount == other.Amount;
        }
    }
}
