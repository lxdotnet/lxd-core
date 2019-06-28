using System.Linq.Expressions;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions.Operators
{
    public class And : LogicalOperator
    {
        public And(AndModel model, ExecutionEngine engine) : base(model, engine) { }

        protected override ExpressionType ExpressionType => ExpressionType.AndAlso;
    }
}
