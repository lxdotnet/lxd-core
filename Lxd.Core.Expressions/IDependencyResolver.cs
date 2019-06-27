
using System;
using System.Collections.Generic;

namespace Lxd.Core.Expressions
{
    public interface IDependencyResolver
    {
        object Resolve(Type t);
        IEnumerable<Type> KnownTypes { get; }
    }
}