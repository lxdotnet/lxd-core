using System.Linq.Expressions;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
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
