
using System;
using System.Reflection;

namespace Lxdn.Core.Aggregates
{
    internal struct IndexAccessor : IAccessor
    {
        private readonly PropertyInfo indexer;
        private readonly int index;
        private readonly object owner;

        public IndexAccessor(PropertyInfo indexer, int index, object owner)
        {
            this.indexer = indexer;
            this.index = index;
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public object GetValue()
        {
            var that = this;
            return Guard.Function(() => that.indexer.GetValue(that.owner, new object[] { that.index }),
                ex => new InvalidOperationException($"Can't get index [{that.index}] of {that.owner}"));
        }

        public void SetValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}