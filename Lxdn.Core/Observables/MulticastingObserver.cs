using System;
using System.Collections.Generic;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Observables
{
    public class MulticastingObserver<TSource> : IObserver<TSource>
    {
        private readonly List<IObserver<TSource>> observers = new List<IObserver<TSource>>();

        public MulticastingObserver<TSource> Add(IObserver<TSource> observer)
        {
            observers.Add(observer);
            return this;
        }

        public void OnNext(TSource value) => observers.SafeForEach(observer => observer.OnNext(value));

        public void OnError(Exception ex) => observers.SafeForEach(observer => observer.OnError(ex));

        public void OnCompleted() => observers.SafeForEach(observer => observer.OnCompleted());
    }
}