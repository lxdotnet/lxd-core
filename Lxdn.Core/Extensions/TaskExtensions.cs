
using System;
using System.Threading.Tasks;

namespace Lxd.Core.Extensions
{
    public static class TaskExtensions
    {
        public static Task<TResult> Within<TResult>(this Task<TResult> task, TimeSpan span)
        {
            return Task.WhenAny(task, Task.Delay(span)).ContinueWith(t =>
            {
                if (!task.IsCompleted)
                    throw new TimeoutException("Could not complete within " + span.Milliseconds + "ms");

                return task.Result;
            });
        }

        public static Task Within(this Task task, TimeSpan span)
        {
            return Task.WhenAny(task, Task.Delay(span)).ContinueWith(t =>
            {
                if (!task.IsCompleted)
                    throw new TimeoutException("Could not complete within " + span.Milliseconds + "ms");
            });
        }
    }
}
