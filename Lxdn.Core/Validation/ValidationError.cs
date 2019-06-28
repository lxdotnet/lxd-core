
using System.Diagnostics;

namespace Lxdn.Core.Validation
{
    [DebuggerDisplay("{Id,nq}")]
    public class ValidationError
    {
        public ValidationError(string id, string message)
        {
            this.Id = id;
            this.Message = message;
        }

        protected ValidationError(ValidationError error) : this(error.Id, error.Message) {}

        public string Id { get; private set; }

        public string Message { get; protected set; }

        public ValidationResult For(OperatorPath path)
        {
            return new ValidationResult(this, path);
        }
    }
}