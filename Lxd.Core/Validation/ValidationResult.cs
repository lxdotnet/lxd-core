
using System.Diagnostics;

namespace Lxd.Core.Validation
{
    [DebuggerDisplay("{Id,nq} of {Path,nq}")]
    public class ValidationResult : ValidationError
    {
        public ValidationResult(ValidationError error, OperatorPath path) : base(error)
        {
            this.Path = path;
        }

        public OperatorPath Path { get; private set; }
        
        public OperatorModelValidationException ToException()
        {
            return new OperatorModelValidationException(this);
        }
    }
}