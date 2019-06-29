
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lxdn.Core.Basics
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory, LazyThreadSafetyMode mode) :
            base(() => Task.Factory.StartNew(taskFactory).Unwrap(), mode) { }

        public AsyncLazy(Func<Task<T>> taskFactory) : this(taskFactory, LazyThreadSafetyMode.ExecutionAndPublication) { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
}