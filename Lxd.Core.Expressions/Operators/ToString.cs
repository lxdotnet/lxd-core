using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using Lxd.Core.Basics;
using Lxd.Core.Expressions.Extensions;
using Lxd.Core.Expressions.Operators.Models;
using Lxd.Core.Expressions.Operators.Models.Output;
using Lxd.Core.Expressions.Utils;

namespace Lxd.Core.Expressions.Operators
{
    public class ToString : Operator
    {
        private readonly Func<Expression> create;

        public ToString(ToStringModel toString, ExecutionEngine logic)
        {
            this.create = () =>
            {
                Operator argument = logic.Operators.CreateFrom(toString.Argument);
                var culture = ParseCulture(toString.Culture, logic);

                return Expression.Call(argument.Expression, ToStringMethod, Expression.Constant(toString.Format), culture);
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }

        private static Expression ParseCulture(string value, ExecutionEngine engine)
        {
            if (string.IsNullOrEmpty(value))
                //return Expression.Constant(CultureInfo.InvariantCulture);
                return Expression.Constant(Thread.CurrentThread.CurrentCulture);

            if (Regex.IsMatch(value, @"^\d+$"))
                return Expression.Constant(CultureInfo.GetCultureInfo(int.Parse(value)));

            if (Regex.IsMatch(value, @"^[A-Za-z]{2}-[A-Z-a-z]{2}$"))
                return Expression.Constant(CultureInfo.GetCultureInfo(value));

            var path = new StructuredPath(value);
            Model model = engine.Models.FirstOrDefault(m => m.Id.Equals(path.Entry));

            if (model != null)
            {
                var property = new Property(value, engine);
                return property.Expression;
            }

            throw new XmlConfigException("Invalid culture: " + value);
        }

        static ToString()
        {
            ToStringMethod = typeof(IFormattable).GetMethod("ToString", new[] { typeof(string), typeof(CultureInfo) });
        }

        private static readonly MethodInfo ToStringMethod;
    }
}
