
namespace Lxd.Core.Expressions.Operators.Custom
{
    public abstract class CustomOperator<TReturn, TRuntimeModel1, TRuntimeModel2> : CustomOperator
    {
        protected CustomOperator(ExecutionEngine logic) : base(logic) {}
        protected abstract TReturn Evaluate(TRuntimeModel1 parameter1, TRuntimeModel2 parameter2);
    }
}