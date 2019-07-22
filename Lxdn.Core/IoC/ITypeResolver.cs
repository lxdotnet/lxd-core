using System;

namespace Lxdn.Core.IoC
{
    public interface ITypeResolver
    {
        object Resolve(Type type);
    }
}
