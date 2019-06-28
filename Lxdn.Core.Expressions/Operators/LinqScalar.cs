
using System.Linq.Expressions;
using Lxd.Core.Expressions.Operators.Models.Linq;

namespace Lxd.Core.Expressions.Operators
{
    [CountOfArguments(1)]
    public class LinqScalar : LinqBase<LinqScalarModel>
    {
        public LinqScalar(LinqScalarModel model, ExecutionEngine engine) : base(model, engine) {}

        public LinqScalar(Expression expression, string verb, ExecutionEngine engine) : base(expression, verb, engine) {}

        protected internal override Expression Create()
        {
            return Expression.Call(this.Method, this.Collection.Expression);
        }
    }
}
