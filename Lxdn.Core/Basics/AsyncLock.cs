
using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{semaphore.CurrentCount}")]
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim semaphore;

        public AsyncLock() : this(1, int.MaxValue) {}

        public AsyncLock(int initialCount, int maxCount)
        {
            this.semaphore = new SemaphoreSlim(initialCount, maxCount);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public async Task<IDisposable> LockAsync(TimeSpan timeout)
        {
            if (await this.semaphore.WaitAsync(timeout).ConfigureAwait(false)) // http://www.infoq.com/news/2013/07/async-await-pitfalls + // http://www.infoq.com/articles/Async-API-Design
                return new Releaser(this);

            throw new TimeoutException("Could not acquire async lock within " + timeout);
        }

        public Task<IDisposable> LockAsync()
        {
            return this.LockAsync(TimeSpan.FromMilliseconds(Timeout.Infinite));
        }

        public IDisposable Lock(TimeSpan timeout) // works, but unfortunately NOT reenterable (in contrast to classical lock(), an attempt to call it on the same thread twice will cause a deadlock)
        {
            if (this.semaphore.Wait(timeout))
                return new Releaser(this);

            throw new TimeoutException("Could not acquire lock within " + timeout);
        }

        private void Release()
        {
            this.semaphore.Release();
        }

        public class Releaser : IDisposable
        {
            private readonly AsyncLock asyncLock;

            public Releaser(AsyncLock asyncLock)
            {
                this.asyncLock = asyncLock;
            }

            public void Dispose()
            {
                this.asyncLock.Release();
            }
        }

        public WaitHandle AwaitableHandle
        {
            get { return this.semaphore.AvailableWaitHandle; }
        }

        public int CurrentCount
        {
            get { return this.semaphore.CurrentCount; }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.semaphore.Dispose();
            }
        }

        ~AsyncLock()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }
    }

    // http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
}
