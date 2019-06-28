using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Exceptions;
using Lxdn.Core.Expressions.Operators.Models.Linq;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    public abstract class LinqBase<TModel> : Operator
        where TModel : LinqBaseModel, new()
    {
        protected LinqBase(TModel model, ExecutionEngine engine)
        {
            this.Collection = engine.Operators.CreateProperty(model.Enumerable);
            var arguments = this.Collection.Type.AsArgumentsOf(typeof(IEnumerable<>));

            if (!arguments.HasValue)
                throw new ExpressionConfigException(this.Collection.Expression + " must be IEnumerable<>");

            this.ArgumentType = arguments.Value.Single();

            // get linq extension method based on the node name:
            this.Method = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.Equals(model.Verb))
                .First(m => m.GetParameters().Count().Equals(CountOfArguments.From(this.GetType())))
                .MakeGenericMethod(this.ArgumentType);
        }

        protected LinqBase(Expression expression, string verb, ExecutionEngine engine)
            : this(new TModel { Verb = verb, Enumerable = expression.ToString() }, engine) {}

        protected Property Collection { get; private set; }

        protected Type ArgumentType { get; private set; }

        protected MethodInfo Method { get; private set; }
    }
}
