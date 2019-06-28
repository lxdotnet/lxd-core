
using System;
using System.Collections.Generic;

namespace Lxdn.Core.Expressions
{
    public interface IDependencyResolver
    {
        object Resolve(Type t);
        IEnumerable<Type> KnownTypes { get; }
    }
}