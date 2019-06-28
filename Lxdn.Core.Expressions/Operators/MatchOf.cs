using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Lxdn.Core.Expressions.Operators.Models.Strings;

namespace Lxdn.Core.Expressions.Operators
{
    public class MatchOf : Operator
    {
        private readonly Func<Expression> create;

        /// <summary>
        /// Generates the expression Regex.Matches(input, pattern).OfType[Match]().Count[] != 0
        /// (semantically same as '{input} is a match of {pattern}')
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="engine"></param>
        public MatchOf(MatchOfModel isMatchOf, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var input =logic.Operators.CreateFrom(isMatchOf.Argument, typeof(string));
                var matches = Expression.Call(Match, input.Expression, Expression.Constant(isMatchOf.Pattern));
                var typed = Expression.Call(OfType, matches);

                var count = typeof(Enumerable).GetMethods()
                    .Where(m => m.Name == "Count")
                    .Single(m => m.GetParameters().Length == 1)
                    .MakeGenericMethod(typeof(Match));

                var n = Expression.Call(count, typed);

                return Expression.MakeBinary(ExpressionType.NotEqual, n, Expression.Constant(0));
            };
        }
        
        protected internal override Expression Create()
        {
            return this.create();
        }

        static MatchOf()
        {
            Match = typeof(Regex).GetMethod("Matches", new[] { typeof(string), typeof(string) });
            OfType = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(typeof(Match));
        }

        private static readonly MethodInfo Match;

        private static readonly MethodInfo OfType;
    }
}