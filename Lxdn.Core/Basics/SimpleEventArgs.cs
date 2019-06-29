using System;
using System.Diagnostics;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{Value}")]
    public class SimpleEventArgs<T> : EventArgs
    {
        public SimpleEventArgs(T t)
        {
            this.Value = t;
        }

        public T Value { get; private set; }
    }

    public delegate void SimpleEventHandler<T>(object sender, SimpleEventArgs<T> args);

    [DebuggerDisplay("{Value}")]
    public class SimpleMutableEventArgs<T> : EventArgs
    {
        public SimpleMutableEventArgs(T t)
        {
            this.Value = t;
        }

        public SimpleMutableEventArgs() { }

        public T Value { get; set; }
    }
}
