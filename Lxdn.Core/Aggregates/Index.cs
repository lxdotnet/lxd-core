using System;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    public class Index : IStep
    {
        private readonly int value;

        public Index(int value)
        {
            this.value = value;
        }

        public Type Type => throw new NotImplementedException();

        public object GetValue(object current)
        {
            throw new NotImplementedException();
        }

        public object InstantiateIn(object owner)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object current, object value)
        {
            throw new NotImplementedException();
        }

        public Expression ToExpression(Expression current)
        {
            throw new NotImplementedException();
        }
    }
}