
using System.Linq;
using Lxdn.Core.Expressions.Operators.Models.Validation.Extensions;
using Lxdn.Core.Validation;

namespace Lxdn.Core.Expressions.Operators.Models.Validation.OfModels
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