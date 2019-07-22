
using System;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.IoC
{
    public class TypeResolver : ITypeResolver
    {
        private readonly ITypeResolver parent;

        private readonly Dictionary<Type, object> map = new Dictionary<Type, object>();

        public TypeResolver(ITypeResolver parent = null)
        {
            this.parent = parent ?? Default;
        }

        private static readonly ITypeResolver Default;

        static TypeResolver()
        {
            Default = new DefaultResolver();
        }

        private class DefaultResolver : ITypeResolver
        {
            public object Resolve(Type t) => Activator.CreateInstance(t);
        }

        public TypeResolver Consider<T>(Type t, T item)
        {
            if (this.map.ContainsKey(t))
                throw new ArgumentException("Already declared: " + t.FullName);

            this.map.Add(t, item);
            return this;
        }

        public TypeResolver Consider<T>(T t) => this.Consider(typeof(T), t);

        public object Resolve(Type t)
        {
            /* todo:
            // B -> D; (B,f); ?D
            this.map
                .Where(mapping => mapping.Key.IsAssignableFrom(t))
                .OrderBy(mapping => mapping.Key.DistanceTo(t)).FirstOrDefault() */ // .AncestryDepthOf()

            return this.map.Keys.FirstOrDefault(t.IsAssignableFrom).IfExists(k => this.map[k]) ?? this.parent.Resolve(t);
            // current implementation just looks for the first candidate, but it should consider them all
            // and then select the best one reasoning for the distance to the 't' in the 
        }

        public TypeResolver Chain() => new TypeResolver(this);
    }
}