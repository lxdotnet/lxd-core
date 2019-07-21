using System;

namespace Lxdn.Core
{
    public static class Guard
    {
        public static TReturn Function<TReturn, TException>(Func<TReturn> logic, Func<Exception, TException> onException)
            where TException : Exception
        {
            try { return logic(); }
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