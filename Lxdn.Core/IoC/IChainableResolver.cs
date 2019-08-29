using System;

namespace Lxdn.Core.IoC
{
    public interface IChainableResolver : ITypeResolver, IDisposable
    {
        IChainableResolver Chain(params object[] dependencies);
    }

    //public class W<T> : IChainableResolver
    //{
    //    ctor

    //}

    //public class Wrapper : IChainableResolver
    //{
    //    private readonly Func<Type, object> resolve;
    //    private readonly Action onDispose;

    //    public Wrapper(Func<Type, object> resolve, Action<object> register, Action onDispose)
    //    {
    //        this.resolve = resolve;
    //        this.onDispose = onDispose;
    //    }

    //    public IChainableResolver Chain(params object[] dependencies) => chain();

    //    public void Dispose() => onDispose();

    //    public object Resolve(Type type) => resolve(type);
    //}
}
