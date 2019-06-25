
using System;

namespace Lxd.Core.Remoting
{
    public abstract class Logic<TContext> : MarshalByRefObject
        where TContext : MarshalByRefObject
    {
        public abstract void Execute(TContext context);
    }
}