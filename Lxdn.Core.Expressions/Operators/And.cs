using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
{
    public class And : LogicalOperator
    {
        public And(AndModel model, ExecutionEngine engine) : base(model, engine) { }

        protected override ExpressionType ExpressionType => ExpressionType.AndAlso;
    }
}
