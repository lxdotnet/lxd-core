using System;
using Lxd.Core.Extensions;

namespace Lxd.Core.Observables
{
    public class Observer<TSource> : IObserver<TSource>
    {
        private readonly Action<TSource> onNext;

        private readonly Action onCompleted;

        private readonly Action<Exception> onError;

        public Observer(Action<TSource> onNext, Action onCompleted, Action<Exception> onError)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
            this.onError = onError;
        }

        public void OnNext(TSource value)
        {
            this.onNext.IfExists(next => next(value));
        }

        public void OnError(Exception exception)
        {
            this.onError.IfExists(error => error(exception));
        }

        public void OnCompleted()
        {
            this.onCompleted.IfExists(completed => completed());
        }
    }
}