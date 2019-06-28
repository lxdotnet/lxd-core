
namespace Lxd.Core.Expressions.Operators.Custom
{
    internal class RuntimeContext : IRuntimeContext
    {
        private readonly object[] models;

        public RuntimeContext(params object[] models)
        {
            this.models = models;
        }

        public TReturn Resolve<TReturn>(IEvaluator<TReturn> promise)
        {
            return promise.From(this.models);
        }
    }
}
