
using System;
using System.Threading;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Lxdn.Core.Basics
{
    public class Sequence<TValue> where TValue : IComparable<TValue>
    {
        private readonly TValue from;
        private readonly TValue to;
        private readonly TValue step;

        public Sequence(TValue from, TValue to, TValue step)
        {
            this.from = from;
            this.to = to;
            this.step = step;
        }
        
        private static readonly Lazy<Func<TValue, TValue, TValue>> Next = new Lazy<Func<TValue, TValue, TValue>>(() =>
        {
            var loopVariable = Expression.Parameter(typeof(TValue), "i");
            var step = Expression.Parameter(typeof(TValue), "step");

            return Expression.Lambda<Func<TValue, TValue, TValue>>(Expression.MakeBinary(ExpressionType.Add, 
                loopVariable, step), loopVariable, step).Compile();

        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public IEnumerable<TValue> Yield()
        {
            Func<TValue, bool> condition = value => step.CompareTo(default(TValue)) > 0
                ? value.CompareTo(this.to) <= 0
                : value.CompareTo(this.to) >= 0;

            for (var i = this.from; condition(i); i = Next.Value(i, step))
                yield return i;
        }
    }
}
