
using System;
using System.Linq.Expressions;
using System.Reflection;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators
{
    public class AnyTypeConverter : Operator
    {
        private readonly Type target;

        private readonly Operator operand;

        public AnyTypeConverter(Operator op, Type target)
        {
            this.target = target;
            this.operand = op;
        }

        static AnyTypeConverter()
        {
            toString = typeof(object).GetMethod("ToString", Type.EmptyTypes);
        }

        private static readonly MethodInfo toString;

        protected internal override Expression Create()
        {
            if (this.target == this.operand.Expression.Type)
            {
                return this.operand.Expression;
            }

            if (typeof(string) == this.target)
            {
                return Expression.Call(this.operand.Expression, toString);
            }
            
            // the sequence is important; changing the sequence breaks the tests;
            if (this.target == typeof(bool))
            {
                return this.operand.Expression.ToBoolean();
            }
            
            if (this.operand.Expression.Type == typeof(string))
            {
                var cast = typeof(StringExtensions).GetMethod("Cast", new[] { typeof(string), typeof(Type) });
                var obj = Expression.Call(cast, this.operand.Expression, Expression.Constant(this.target));
                return Expression.Convert(obj, this.target);
            }
            
            return Expression.Convert(this.operand.Expression, this.target);
        }
    }

    public class AnyTypeConverter<TReturn> : Operator
    {
        private readonly AnyTypeConverter converter;

        internal AnyTypeConverter(Operator op)
        {
            this.converter = new AnyTypeConverter(op, typeof(TReturn));
        }

        protected internal override Expression Create()
        {
            return this.converter.Create();
        }
    }
}