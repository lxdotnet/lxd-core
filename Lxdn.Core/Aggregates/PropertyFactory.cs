
using System;

using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using Lxdn.Core.Aggregates.Models;

namespace Lxdn.Core.Aggregates
{
    public class PropertyFactory<TReturn>
    {
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
            return new Property<TReturn>(model, path.Steps);
        }
    }
}