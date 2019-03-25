using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit
{
    public class Throttle<T> where T : EventArgs
    {
        public static EventHandler<T> Subscribe(Action<object, T> action, int millisecondsDelay = 200)
        {
            return new EventHandler<T>(new Throttle<T>(action, millisecondsDelay).Handler);
        }

        private long invokeCount = 0;

        public Action<object, T> Handler { get; }

        private Throttle(Action<object, T> action, int millisecondsDelay)
        {
            Handler = async (s, e) => {
                var id = System.Threading.Interlocked.Increment(ref invokeCount);
                await System.Threading.Tasks.Task.Delay(millisecondsDelay);
                var current = System.Threading.Interlocked.Read(ref invokeCount);
                if (current == id)
                {
                    action?.Invoke(s, e);
                }
            };
        }
    }
}
