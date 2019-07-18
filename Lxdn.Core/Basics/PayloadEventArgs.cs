using System;
using System.Diagnostics;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{Value}")]
    public class PayloadEventArgs<TPayload> : EventArgs
    {
        public PayloadEventArgs(TPayload payload)
        {
            this.Payload = payload;
        }

        public TPayload Payload { get; private set; }
    }

    public delegate void PayloadEventHandler<TPayload>(object sender, PayloadEventArgs<TPayload> args);
}
