using System;

namespace Lxd.Core.Injection
{
    public interface IInjectionFactory
    {
        object Create(object source, Type target);
    }
}