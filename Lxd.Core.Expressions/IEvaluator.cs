
using System.Linq.Expressions;

namespace Lxd.Core.Expressions
{
    public interface IEvaluator<out TReturn>
    {
        TReturn From(params object[] modelInstances);
        LambdaExpression Logic { get; }
    }
}