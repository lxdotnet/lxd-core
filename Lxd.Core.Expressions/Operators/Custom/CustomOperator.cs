
using System;
using System.Reflection;
using System.Linq.Expressions;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Custom
{
    public abstract class CustomOperator : Operator
    {
        private readonly Func<Expression> evaluate;

        protected CustomOperator(ExecutionEngine logic)
        {
            var method = this.GetType().GetMethod("Evaluate", BindingFlags.NonPublic | BindingFlags.Instance);
            this.evaluate = () => method.Call(this, new RuntimeInjector(logic.Models));
        }

        protected internal override Expression Create()
        {
            return this.evaluate();
        }
    }

    public abstract class CustomOperator<TReturn> : CustomOperator
    {
        protected CustomOperator(ExecutionEngine logic) : base(logic) { }
        protected abstract TReturn Evaluate();
    }

    public abstract class CustomOperator<TReturn, TRuntimeModel> : CustomOperator
    {
        protected CustomOperator(ExecutionEngine logic) : base(logic) {}
        protected abstract TReturn Evaluate(TRuntimeModel parameter);
    }
}
