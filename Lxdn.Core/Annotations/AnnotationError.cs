#if NETFULL

namespace Lxdn.Annotations
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;

    [DebuggerDisplay("{Attribute.GetType().Name,nq}")]
    public class AnnotationError
    {
        public AnnotationError(ValidationAttribute attribute, string message)
        {
            this.Attribute = attribute;
            this.Message = message;
        }

        protected AnnotationError(AnnotationError error) : this(error.Attribute, error.Message) {}

        public ValidationAttribute Attribute { get; }

        public string Message { get; protected set; }

        public AnnotationResult For(Path path)
        {
            return new AnnotationResult(this, path);
        }
    }
}

#endif