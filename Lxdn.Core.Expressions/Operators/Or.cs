using System.Linq.Expressions;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions.Operators
{
    public class Or : LogicalOperator
    {
        public Or(OrModel model, ExecutionEngine engine) : base(model, engine) { }

        protected override ExpressionType ExpressionType
        {
            //get { return ExpressionType.Or; }
            get { return ExpressionType.OrElse; }
        }
    }
}
