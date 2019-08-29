
using System;
using System.Linq;
using System.Reflection;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.IoC
{
    public class Resolver : IChainableResolver
    {
        private readonly DependencyMap map;

        internal Resolver(DependencyMap knownDependencies = null)
        {
            map = knownDependencies ?? new DependencyMap();
        }

        public Resolver(params object[] initial) : this(new DependencyMap(initial)) { }

        public IChainableResolver Chain(params object[] dependencies) => new Resolver(map.Clone().Consider(dependencies));

        public object Resolve(Type t)
        {
            object construct(ConstructorInfo ctor) =>
                ctor.Invoke(ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray());

            return map.TryGet(t) 
                ?? t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .ThrowIf(c => c.Length != 1, c => new ArgumentException($"Unexpected count of public constructors of {t.FullName}. Expected 1."))
                    .Single().IfExists(construct);
        }

        public void Dispose() { }
    }
}
