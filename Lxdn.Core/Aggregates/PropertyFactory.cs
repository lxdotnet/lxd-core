
using System;
using System.Collections.Generic;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using Lxdn.Core.Aggregates.Models;
using System.Linq;

namespace Lxdn.Core.Aggregates
{
    public class PropertyFactory<TReturn>
    {
        public Property<TReturn> CreateFrom(Model root, IEnumerable<IStepModel> stepModels)
        {
            var steps = new List<IStep>();
            Type currentType() => steps.LastOrDefault()?.Type ?? root.Type;

            stepModels.Aggregate(steps, (_steps, step) =>
                _steps.Push(StepFactory.Of(currentType()).Create(step)));

            return new Property<TReturn>(root, steps);
        }

        public Property<TReturn> CreateFrom(Type root, string pathLiteral)
        {
            root.ThrowIfDefault();

            if (string.IsNullOrWhiteSpace(pathLiteral))
                throw new ArgumentNullException(nameof(pathLiteral));

            var path = PathModel.Parse(pathLiteral);
            return CreateFrom(root, path);
        }

        public Property<TReturn> CreateFrom(Type root, PathModel path)
        {
            var model = new Model(path.Root, root.ThrowIfDefault());
            return CreateFrom(model, path.Steps);
        }

        public Property<TReturn> CreateFrom(Model root) => CreateFrom(root, Enumerable.Empty<IStepModel>());
    }
}