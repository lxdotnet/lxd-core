
using System;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class Property<TValue> : IEnumerable<Step>
    {
        private readonly IEnumerable<Step> steps = new List<Step>();

        public static Property<TValue> From(Type rootType, string pathLiteral)
        {
            var path = PathModel.Parse(pathLiteral);
            var root = new Model(path.Root, rootType);

            return new Property<TValue>(root, path.Tokens);
        }

        internal Property(Model root, IEnumerable<Step> steps)
        {
            this.Root = root;
            this.steps = steps;
        }

        public Property(Model root, IEnumerable<string> tokens)
        {
            Root = root;

            tokens.Aggregate((IList<Step>)this.steps, (steps, token) =>
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