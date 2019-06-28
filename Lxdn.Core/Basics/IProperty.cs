using System;
using System.Linq.Expressions;

namespace Lxdn.Core.Basics
{
    public interface IProperty
    {
        Type Type { get; }
        Expression ToExpression(Expression entry);
        IPropertyAccessor CreateAccessor(object entry);
    }
}
