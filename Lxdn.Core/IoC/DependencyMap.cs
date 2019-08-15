
using System;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.IoC
{
    internal class DependencyMap
    {
        private readonly Dictionary<Type, object> map = new Dictionary<Type, object>();

        public DependencyMap(IEnumerable<object> dependencies = null) => Consider(dependencies ?? Enumerable.Empty<object>());

        public DependencyMap Consider(IEnumerable<object> dependencies)
        {
            dependencies
                .Select(dependency => new { Type = dependency.GetType(), Value = dependency })
                .Aggregate(map, (m, dependency) => m.Push(dependency.Type, dependency.Value, () => m[dependency.Type] = dependency.Value));

            return this;
        }

        public DependencyMap Clone() => new DependencyMap(map.Values);

        public object TryGet(Type t)
        {
            var result = map
                .Where(registration => t.IsAssignableFrom(registration.Key))
                .ThrowIf(x => x.Count() > 1, x => new InvalidOperationException($"Ambiguous candidates for {t.FullName} found"))
                .SingleOrDefault()
                .IfExists(r => r.Value);

            return result;
        }
    }
}
