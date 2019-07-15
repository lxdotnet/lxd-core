
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class Property<TValue> : IEnumerable<PropertyInfo>
    {
        private readonly IEnumerable<PropertyInfo> properties;

        public Property(Type root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var tokens = path.SplitBy(".").ToList();

            this.Root = new Model(tokens[0], root);

            this.properties = tokens.Skip(1)
                .Aggregate((IList<PropertyInfo>)new List<PropertyInfo>(), (properties, token) =>
                    properties.Push((properties.LastOrDefault()?.PropertyType ?? Root.Type).GetProperty(token)
                        .ThrowIfDefault(() => new ArgumentException("Invalid property: " + token))));
        }

        internal Property(Model root, IEnumerable<PropertyInfo> properties)
        {
            this.Root = root;
            this.properties = properties;
        }

        public Model Root { get; }

        public IEnumerator<PropertyInfo> GetEnumerator() => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PropertyAccessor<TValue> Of(object root) => new PropertyAccessor<TValue>(this, root);

        public override string ToString()
        {
            return Root.Id.Once().Concat(properties.Select(property => property.Name))
                .Agglutinate(".");
        }
    }
}