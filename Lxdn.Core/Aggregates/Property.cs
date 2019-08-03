
using System;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using Lxdn.Core.Aggregates.Models;

namespace Lxdn.Core.Aggregates
{
    public class Property<TReturn> : IEnumerable<IStep>
    {
        private readonly List<IStep> steps = new List<IStep>();

        public static PropertyFactory<TReturn> Create => new PropertyFactory<TReturn>();

        public Property(Model root, IEnumerable<IStepModel> steps)
        {
            Root = root;
            steps.Aggregate(this.steps, (_steps, step) =>
                _steps.Push(StepFactory.Of(Type).Create(step)));
        }

        public Model Root { get; }

        public Type Type => steps.LastOrDefault()?.Type ?? Root.Type;

        public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PropertyAccessor<TReturn> Of(object root) => new PropertyAccessor<TReturn>(this, root);

        public Expression ToExpression(Expression parameter) =>
            this.Aggregate(parameter, (current, step) => step.ToExpression(current));

        public override string ToString()
        {
            return Root.Id.Once().Concat(steps.Select(step => step.ToString()))
                .Agglutinate(".");
        }
    }
}