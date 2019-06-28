
using System;

namespace Lxdn.Core.Remoting
{
    public abstract class Logic<TContext> : MarshalByRefObject
        where TContext : MarshalByRefObject
    {
        public abstract void Execute(TContext context);
    }
}