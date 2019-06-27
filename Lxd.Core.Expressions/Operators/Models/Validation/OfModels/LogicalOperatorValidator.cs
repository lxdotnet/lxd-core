
using System.Linq;
using Lxd.Core.Expressions.Operators.Models.Validation.Extensions;
using Lxd.Core.Validation;

namespace Lxd.Core.Expressions.Operators.Models.Validation.OfModels
{
    public class LogicalOperatorValidator : IValidator<LogicalOperatorModel>
    {
        public bool IsInvalid(LogicalOperatorModel model)
        {
            return model.Operands.Count() < 2;
        }

        public ValidationError GetError(LogicalOperatorModel model)
        {
            return GenericMessages.LogicalOperatorHasTooFewMembers.ToError();
        }
    }
}