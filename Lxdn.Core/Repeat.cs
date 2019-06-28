using System;

namespace Lxdn.Core
{
    public class Repeat
    {
        public static void OnException(Action action, Func<Exception, bool> shouldRepeatOn, int times)
        {
            var counter = 0;

            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    if (!shouldRepeatOn(ex) || counter >= times)
                        throw;

                    counter++;
                }
            }
        }

        public static void OnException(Action action, int times) => OnException(action, ex => true, times); // repeat on all exceptions
    }
}