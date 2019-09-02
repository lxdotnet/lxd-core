
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    [DebuggerDisplay("[{index,nq}]")]
    public class Index : IStep
    {        
        private readonly int index;

        private readonly PropertyInfo indexer;

        internal Index(Type indexable, int index)
        {            
            this.index = index;

            bool HasNumericIndexer(PropertyInfo property)
            {
                var indices = property.GetIndexParameters();
                if (indices.Length == 1 && indices.Single().ParameterType.IsOneOf(typeof(int), typeof(long)))
                    return true;

                return false;
            }

            indexer = indexable
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(HasNumericIndexer)
                .ThrowIf(indexers => indexers.Count() != 1, x => new Exception($"{indexable}: indexer missing or ambiguous."))
                .Single();
        }

        public Type Type => indexer.PropertyType;

        public object GetValue(object current) => Guard.Function(() => 
            indexer.GetValue(current.ThrowIfDefault(), new object[] { index }), 
            ex => new InvalidOperationException($"Can't get index [{index}] of {current}"));

        public object InstantiateIn(object owner)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object current, object value)
        {
            throw new NotImplementedException();
        }

        public Expression ToExpression(Expression current) => Expression.MakeIndex(current, indexer, new [] { Expression.Constant(index) });

        public override string ToString() => $"[{index}]";
    }
}