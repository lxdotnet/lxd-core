using System;

namespace Lxdn.Core.IoC
{
    public interface IChainableResolver : ITypeResolver, IDisposable
    {
        IChainableResolver Chain(params object[] dependencies);
    }
}
