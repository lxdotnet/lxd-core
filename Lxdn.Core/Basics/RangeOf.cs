using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{Min} - {Max}")]
    public class RangeOf<TValue> : IEquatable<RangeOf<TValue>>
        where TValue : struct, IComparable<TValue>
    {
        public TValue Min { get; set; }
        public TValue Max { get; set; }

        public static RangeOf<TValue> Zero => new RangeOf<TValue> { Min = default(TValue), Max = default(TValue) };

        public bool Equals(RangeOf<TValue> other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            return Equals(this.Min, other.Min) && Equals(this.Max, other.Max);
        }

        public override bool Equals(object obj)
        {
            var other = obj as RangeOf<TValue>;
            return !object.ReferenceEquals(other, null) && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.HashUsing(range => range.Min, range => range.Max);
        }

        public bool Contains(TValue value)
        {
            return value.CompareTo(this.Min) >= 0 && value.CompareTo(this.Max) <= 0;
        }

        public static bool operator !=(RangeOf<TValue> left, RangeOf<TValue> right)
        {
            return !object.Equals(left, right);
        }

        public static bool operator ==(RangeOf<TValue> left, RangeOf<TValue> right)
        {
            return object.Equals(left, right);
        }

        public IEnumerable<TValue> Step(TValue step)
        {
            var sequence = new Sequence<TValue>(this.Min, this.Max, step);
            return sequence.Yield();
        }

        public static RangeOf<TValue> From(TValue from, TValue to)
        {
            return new RangeOf<TValue> { Min = from, Max = to };
        }

        public override string ToString()
        {
            return $"{Min} - {Max}";
        }

        public RangeOf<TOther> Transform<TOther>(Func<TValue, TOther> transform)
            where TOther : struct, IComparable<TOther>
        {
            return RangeOf<TOther>.From(transform(Min), transform(Max));
        }
    }
}