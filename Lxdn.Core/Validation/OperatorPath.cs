using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Lxd.Core.Extensions;

namespace Lxd.Core.Validation
{
    public class OperatorPath : IEnumerable<OperatorStep>
    {
        private readonly IReadOnlyCollection<OperatorStep> steps;

        public OperatorStep Tail => this.steps.LastOrDefault();

        public OperatorPath(IEnumerable<OperatorStep> steps)
        {
            this.steps = new List<OperatorStep>(steps).AsReadOnly();
        }

        public IEnumerator<OperatorStep> GetEnumerator()
        {
            return this.steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public OperatorPath TakeIndex(int index)
        {
            return new OperatorPath(this.steps.With(new CollectionMember(this.Tail.Property, index)));
        }

        public OperatorPath GrowBy(OperatorStep step)
        {
            return new OperatorPath(this.steps.With(step));
        }

        public static OperatorPath Empty => new OperatorPath(Enumerable.Empty<OperatorStep>());

        public bool IsEmpty()
        {
            return !this.steps.Any();
        }

        public override string ToString()
        {
            return this.steps.Select(step => step.ToString())
                .Aggregate(new StringBuilder(), (builder, step) => builder.Append(step)).ToString();
        }
    }
}