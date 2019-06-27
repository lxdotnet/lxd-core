
#if NETFULL

namespace Lxd.Annotations
{
    using System.Diagnostics;

    [DebuggerDisplay("{Attribute.GetType().Name,nq}: {Path.ToString(),nq}")]
    public class AnnotationResult : AnnotationError
    {
        public AnnotationResult(AnnotationError error, Path path) : base(error)
        {
            this.Path = path;
        }

        public Path Path { get; private set; }
    }
}

#endif