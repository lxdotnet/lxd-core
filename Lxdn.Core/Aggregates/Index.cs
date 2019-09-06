
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("[{index,nq}]")]
    internal class Index : IStep
    {        
        private readonly int index;

        private readonly PropertyInfo indexer;

        internal Index(PropertyInfo indexer, int index)
        {            
            this.index = index;
            this.indexer = indexer;
        }

        public Type Type => indexer.PropertyType;

        public IAccessor Of(object owner) => new IndexAccessor(indexer, index, owner);

        public Expression ToExpression(Expression current) => Expression.MakeIndex(current, indexer, new [] { Expression.Constant(index) });

        public override string ToString() => $"[{index}]";
    }
}