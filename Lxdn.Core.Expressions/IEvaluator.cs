
using System.Linq.Expressions;

namespace Lxdn.Core.Expressions
{
    public interface IEvaluator<out TReturn>
    {
        TReturn Evaluate(params object[] modelInstances);
        LambdaExpression Logic { get; }
    }
}