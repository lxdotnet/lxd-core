using System;
using System.Threading.Tasks;

namespace Lxdn.Core
{
    public static class Guard
    {
        /// <summary>
        /// NOT to be used with tasks (won't catch exception)
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="logic"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static TReturn Function<TReturn, TException>(Func<TReturn> logic, Func<Exception, TException> onException)
            where TException : Exception
        {
            try { return logic(); }
            catch (Exception inner) { throw onException(inner); }
        }

        public static async Task<TReturn> Task<TReturn, TException>(Func<Task<TReturn>> task, Func<Exception, TException> onException)
            where TException : Exception
        {
            try { return await task(); }
            catch (Exception inner) { throw onException(inner); }
        }

        public static async Task Task<TReturn, TException>(Func<Task> task, Func<Exception, TException> onException)
            where TException : Exception
        {
            try { await task(); }
            catch (Exception inner) { throw onException(inner); }
        }

        public static void Action<TException>(Action logic, Func<Exception, TException> onException)
            where TException : Exception
        {
            try { logic(); }
            catch (Exception ex) { throw onException(ex); }
        }
    }
}