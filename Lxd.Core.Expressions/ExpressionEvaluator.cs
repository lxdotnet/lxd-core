
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Lxd.Core.Expressions
{
    [DebuggerDisplay("{Logic}")]
    public class ExpressionEvaluator<TReturn> : IEvaluator<TReturn>
    {
        private readonly Func<object[], TReturn> evaluate;

        public ExpressionEvaluator(LambdaExpression lambda)
        {
            this.Logic = lambda;

            var logic = this.Logic.Compile();
            this.evaluate = parameters => (TReturn)logic.DynamicInvoke(parameters);
        }

        public LambdaExpression Logic { get; }

        public TReturn From(params object[] modelInstances)
        {
            return this.evaluate(modelInstances);
        }
    }
}