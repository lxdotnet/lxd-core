
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
        public Property<TReturn> From(Model root, IEnumerable<IStepModel> stepModels)
        {
            var steps = new List<IStep>();
            Type currentType() => steps.LastOrDefault()?.Type ?? root.Type;

            stepModels.Aggregate(steps, (_steps, step) =>
                _steps.Push(StepFactory.Of(currentType()).Create(step)));

            return new Property<TReturn>(root, steps);
        }

        public Property<TReturn> From(Type root, string pathLiteral)
        {
            root.ThrowIfDefault();

            if (string.IsNullOrWhiteSpace(pathLiteral))
                throw new ArgumentNullException(nameof(pathLiteral));

            var path = PathModel.Parse(pathLiteral);
            return From(root, path);
        }

        public Property<TReturn> From(Type root, PathModel path)
        {
            var model = new Model(path.Root, root.ThrowIfDefault());
            return From(model, path.Steps);
        }
    }
}