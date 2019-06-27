
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Lxd.Core.Extensions;

namespace Lxd.Core.Expressions
{
    public class OperatorNamespaceMapper : IEnumerable<OperatorRootAttribute>
    {
        private readonly IEnumerable<OperatorSource> sources;

        public OperatorNamespaceMapper(IEnumerable<OperatorSource> sources)
        {
            this.sources = sources;
        }

        public string Map(Type model)
        {
            var root = this.FirstOrDefault(candidate => model.Namespace.IfExists(ns => 
                ns.StartsWith(candidate.Value.IfExists(v => v.Namespace))));

            return root != null
                ? model.Namespace.IfExists(ns => ns.Replace(root.Value.Namespace, root.As))
                : model.Namespace;
        }

        public IEnumerator<OperatorRootAttribute> GetEnumerator()
        {
            return this.sources
                .Select(source => source.Assembly)
                .SelectMany(assembly => assembly.GetCustomAttributes<OperatorRootAttribute>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
