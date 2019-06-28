
namespace Lxdn.Core.Expressions.Operators.Custom
{
    public interface IRuntimeContext
    {
        TReturn Resolve<TReturn>(IEvaluator<TReturn> promise);
    }
}