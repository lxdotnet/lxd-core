
using System;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    public interface IStep
    {
        object GetValue(object current);
        void SetValue(object current, object value);
        Type Type { get; }
        object InstantiateIn(object owner);
        Expression ToExpression(Expression current);
    }
}