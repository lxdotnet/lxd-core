
using System;
using System.Linq;
using System.Collections.Generic;

using Lxd.Core.Extensions;

namespace Lxd.Core.Expressions
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver parent;

        private readonly Dictionary<Type, Func<object>> map;

        private DependencyResolver(IDependencyResolver parent)
        {
            this.parent = parent;
            this.map = new Dictionary<Type, Func<object>>();
        }

        public DependencyResolver() : this(Empty) {}

        private static readonly IDependencyResolver Empty;

        static DependencyResolver()
        {
            Empty = new EmptyResolver();
        }

        private class EmptyResolver : IDependencyResolver
        {
            public object Resolve(Type t)
            {
                return null;
            }

            public IEnumerable<Type> KnownTypes
            {
                get { return Enumerable.Empty<Type>(); }
            }
        }

        public DependencyResolver Consider<T>(Type t, Func<T> creator)
        {
            if (this.map.ContainsKey(t))
                throw new ArgumentException("Already declared: " + t.FullName);

            this.map.Add(t, () => creator());
            return this;
        }

        public DependencyResolver Consider<T>(Func<T> creator)
        {
            return this.Consider(typeof(T), creator);
        }

        public object Resolve(Type t)
        {
            /* todo:
            // B -> D; (B,f); ?D
            this.map
                .Where(mapping => mapping.Key.IsAssignableFrom(t))
                .OrderBy(mapping => mapping.Key.DistanceTo(t)).FirstOrDefault() */ // .AncestryDepthOf()

            return this.parent.Resolve(t) ??
                this.map.Keys.FirstOrDefault(t.IsAssignableFrom).IfExists(k => this.map[k]());
            // current implementation just looks for the first candidate, but it should consider them all
            // and then select the best one reasoning for the distance to the 't' in the 
        }

        public IEnumerable<Type> KnownTypes
        {
            get { return this.parent.KnownTypes.Union(this.map.Keys); }
        }

        public DependencyResolver Chain()
        {
            return new DependencyResolver(this);
        }
    }
}