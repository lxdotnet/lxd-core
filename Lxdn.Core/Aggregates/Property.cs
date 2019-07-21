
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using System.Linq.Expressions;

namespace Lxdn.Core.Aggregates
{
    public class Property<TValue> : IEnumerable<Step>
    {
        private readonly IEnumerable<Step> steps = new List<Step>();

        public Property(Type root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var tokens = path.SplitBy(".").ToList();

            Root = new Model(tokens[0], root);

            tokens.Skip(1).Aggregate((IList<Step>)this.steps, (steps, token) =>
                steps.Push(StepFactory.Of(Type).CreateStep(token)));
        }

        internal Property(Model root, IEnumerable<Step> steps)
        {
            this.Root = root;
            this.steps = steps;
        }

        public Property(Model root, IEnumerable<string> tokens)
        {
            Root = root;

            tokens.Skip(1).Aggregate((IList<Step>)this.steps, (steps, token) =>
                steps.Push(StepFactory.Of(Type).CreateStep(token)));
        }

        public Model Root { get; }

        public Type Type => steps.LastOrDefault()?.Type ?? Root.Type;

        public IEnumerator<Step> GetEnumerator() => steps.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PropertyAccessor<TValue> Of(object root) => new PropertyAccessor<TValue>(this, root);

        public Expression ToExpression(Expression parameter) =>
            this.Aggregate(parameter, (current, step) => step.ToExpression(current));

        public override string ToString()
        {
            return Root.Id.Once().Concat(steps.Select(step => step.ToString()))
                .Agglutinate(".");
        }
    }
}