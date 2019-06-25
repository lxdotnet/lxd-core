using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lxd.Core.Observables
{
    public class Observable<TSource>
    {
        private readonly Action<IObserver<TSource>, CancellationToken> scenario;

        private readonly MulticastingObserver<TSource> multicast = new MulticastingObserver<TSource>();

        private Observable(Action<IObserver<TSource>, CancellationToken> scenario)
        {
            this.scenario = scenario;
        }

        public static Observable<TSource> Create(Action<IObserver<TSource>, CancellationToken> scenario)
        {
            return new Observable<TSource>(scenario);
        }

        public Observable<TSource> Subscribe(IObserver<TSource> observer)
        {
            multicast.Add(observer);
            return this;
        }

        public Observable<TSource> Subscribe(Action<TSource> onNext, Action onCompleted = null, Action<Exception> onError = null)
            => Subscribe(new Observer<TSource>(onNext, onCompleted, onError));

        public async Task Feed(CancellationToken cancel = default(CancellationToken))
        {
            var awaitable = new AwaitableObserver<TSource>();
            multicast.Add(awaitable);
            scenario(multicast, cancel);
            await awaitable;
        }
    }
}
