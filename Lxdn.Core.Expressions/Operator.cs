using System;
using System.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using Lxdn.Core.Expressions.Operators;

namespace Lxdn.Core.Expressions
{
    //[DebuggerDisplay("{Expression.DebugView}")]
    [DebuggerDisplay("{Expression}")]
    public abstract class Operator
    {
        protected Operator()
        {
            this.expression = new Lazy<Expression>(this.Create);
        }

        private readonly Lazy<Expression> expression;

        public Expression Expression => this.expression.Value;

        protected internal abstract Expression Create();

        public IEvaluator<TReturn> ToEvaluator<TReturn>(IEnumerable<Model> models)
        {
            var op = typeof(TReturn) == this.Expression.Type ? this : this.As<TReturn>();
            //var parameters = models?.Select(m => m.AsParameter()) ?? op.Expression.GuessParameters();
            var parameters = models.Select(model => model.AsParameter());
            return new ExpressionEvaluator<TReturn>(Expression.Lambda(op.Expression, parameters));
        }

        public AnyTypeConverter<TReturn> As<TReturn>()
        {
            return new AnyTypeConverter<TReturn>(this);
        }
        
        public AnyTypeConverter As(Type desired)
        {
            return new AnyTypeConverter(this, desired);
        }

        //public Javascript ToJavascript(params Model[] models)
        //{
        //    return this.Expression.ToJScript(models);
        //}

        public static implicit operator Expression(Operator op)
        {
            return op.IfExists(o => o.Expression);
        }
    }
}
