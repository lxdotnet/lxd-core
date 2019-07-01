
// obsolete!!! candidate for removal

//using System;
//using System.Linq;
//using System.Threading;
//using System.Reflection;
//using System.Linq.Expressions;
//using Lxdn.Core.Expressions.Operators.Models.Output;
//using Lxdn.Core.Extensions;

//namespace Lxdn.Core.Expressions.Operators
//{
//    /// <summary>
//    /// Given a format string with placeholders, replaces them
//    /// with their string representation: either in context of thread's UI culture (if IFormattable)
//    /// or with simply ToString(). Null for reference types is replaced with '(null)'. The fraction of 
//    /// all decimals is stripped (s. to-do)
//    /// </summary>
//    public class StringFormatEx : Operator
//    {
//        private readonly Func<Expression> create;

//        public StringFormatEx(StringFormatExModel model, ExecutionEngine logic)
//        {
//            this.create = () =>
//            {
//                var format = logic.Operators.CreateFrom(model.Format).Expression;

//                return model.Placeholders
//                    .Select(ph => new { Pattern = Expression.Constant(ph.Id), Argument = logic.Operators.CreateFrom(ph.Operator).Expression })
//                    .Select(ph => new { ph.Pattern, Argument = ph.Argument.Type.IsValueType ? Expression.Convert(ph.Argument, typeof(object)) : ph.Argument }) // consider boxing for value types
//                    .Select(ph => new { ph.Pattern, Argument = (Expression)Expression.Call(null, TryFormat, ph.Argument, CurrentUICulture) }) // try format arguments
//                    .Aggregate(format, (current, ph) => Expression.Call(current, Replace, ph.Pattern, ph.Argument));
//            };
//        }

//        protected internal override Expression Create()
//        {
//            return this.create();
//        }

//        static StringFormatEx()
//        {
//            Replace = typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) });
//            TryFormat = typeof(ObjectExtensions).GetMethod("TryFormat");
//            CurrentThread = Expression.Property(null, typeof(Thread), "CurrentThread");
//            CurrentUICulture = Expression.Property(CurrentThread, "CurrentUICulture");
//        }

//        private static readonly MethodInfo TryFormat;

//        private static readonly MethodInfo Replace;

//        private static readonly MemberExpression CurrentThread;

//        private static readonly MemberExpression CurrentUICulture;
//    }
//}
