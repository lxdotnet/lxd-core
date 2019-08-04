
using System;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class Property<TReturn> : IEnumerable<IStep>
    {
        private readonly List<IStep> steps;

        public static PropertyFactory<TReturn> Create => new PropertyFactory<TReturn>();

        public Property(Model root, IEnumerable<IStep> steps)
        {
            Root = root;
            Type = steps.LastOrDefault()?.Type ?? root.Type;

            this.steps = new List<IStep>(steps);
        }

        public Model Root { get; }

        public Type Type { get; }

        public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PropertyAccessor<TReturn> Of(object root) => new PropertyAccessor<TReturn>(this, root);

        public Expression ToExpression(Expression parameter) =>
            this.Aggregate(parameter, (current, step) => step.ToExpression(current));

        public override string ToString() => steps.Aggregate(Root.Id, (path, step) => path + step); // not used in the logic, so the performance is sacrificed for the sake of better readability
    }

    public class Property : Property<object>
    {
        public Property(Model root, IEnumerable<IStep> steps) : base(root, steps) { }

        public Property GrowBy(IStep step) => new Property(Root, this.With(step));
    }
}