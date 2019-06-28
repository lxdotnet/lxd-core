using System;

namespace Lxdn.Core
{
    public static class Guard
    {
        public static TReturn Function<TReturn, TException>(Func<TReturn> logic, Func<Exception, TException> onException)
            where TException : Exception
        {
            try
            {
                return logic();
            }
            catch (Exception inner)
            {
                throw onException(inner);
            }
        }
    }
}