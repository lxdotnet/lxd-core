
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class PropertyPath : IEnumerable<PropertyInfo>
    {
        private readonly IEnumerable<PropertyInfo> properties;

        private PropertyPath(Model root)
        {
            this.Root = root;
        }

        public PropertyPath(Model root, string path) : this(root)
        {
            this.properties = path.SplitBy(".").Skip(1)
                .Aggregate((IList<PropertyInfo>)new List<PropertyInfo>(), (properties, token) =>
                    properties.Push((properties.LastOrDefault()?.PropertyType ?? Root.Type).GetProperty(token)
                        .ThrowIfDefault(() => new ArgumentException("Invalid property: " + token))));
        }

        internal PropertyPath(Model root, IEnumerable<PropertyInfo> properties) : this(root)
        {
            this.properties = properties;
        }

        public Model Root { get; }

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PropertyAccessor Of(object root)
        {
            return new PropertyAccessor(this, root);
        }

        public override string ToString()
        {
            return Root.Id.Once().Concat(properties.Select(property => property.Name))
                .Agglutinate(".");
        }
    }
}