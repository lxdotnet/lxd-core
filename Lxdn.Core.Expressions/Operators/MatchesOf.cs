
using System;
using System.Linq.Expressions;
using System.Reflection;
using Lxdn.Core.Expressions.Operators.Models.Strings;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    public class MatchesOf : Operator
    {
        private readonly Func<Expression> create;

        public MatchesOf(MatchesOfModel matchesOf, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var source = logic.Operators.CreateFrom(matchesOf.Argument, typeof(string));
                return Expression.Call(StaticMethod, source.Expression, Expression.Constant(matchesOf.Pattern, typeof(string)));
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }

        static MatchesOf()
        {
            StaticMethod = typeof(StringExtensions).GetMethod("MatchesOf", new[] { typeof(string), typeof(string) });
        }

        private static readonly MethodInfo StaticMethod;
    }
}
