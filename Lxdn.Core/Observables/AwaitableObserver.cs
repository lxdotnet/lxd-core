using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lxdn.Core.Observables
{
    public class AwaitableObserver<TSource> : IObserver<TSource>
    {
        private readonly TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();

        public void OnNext(TSource value) { }

        public void OnError(Exception ex) => completion.SetException(ex);

        public void OnCompleted() => completion.SetResult(true);

        public TaskAwaiter<bool> GetAwaiter() => completion.Task.GetAwaiter(); // makes it be await-able
    }
}