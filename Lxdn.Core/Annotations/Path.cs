using Lxdn.Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lxdn.Annotations
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class Path : IEnumerable<IStep>
    {
        private readonly List<IStep> steps;

        internal Path(IEnumerable<IStep> steps) : this()
        {
            this.steps.AddRange(steps);
        }

        private Path()
        {
            this.steps = new List<IStep>();
        }

        public static Path Empty => new Path();

        public static Path Begin(object root)
        {
            return Path.Empty.GrowBy(new Root(root));
        }

        internal Path GrowBy(IStep step)
        {
            return new Path(this.steps.With(step));
        }

        public IEnumerator<IStep> GetEnumerator()
        {
            return this.steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return this.steps.Aggregate(new StringBuilder(), (path, step) => 
                path.Append(step)).ToString();
        }
    }
}