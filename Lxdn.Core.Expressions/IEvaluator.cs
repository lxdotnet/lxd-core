
using System.Linq.Expressions;

namespace Lxdn.Core.Expressions
{
    public interface IEvaluator<out TReturn>
    {
        TReturn From(params object[] modelInstances);
        LambdaExpression Logic { get; }
    }
}