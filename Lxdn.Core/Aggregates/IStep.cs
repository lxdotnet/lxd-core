
using System;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    public interface IStep
    {
        Type Type { get; }
        IAccessor Of(object owner);
        Expression ToExpression(Expression current);
    }
}