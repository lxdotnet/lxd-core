
using System;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Extensions;

namespace Lxdn.Core
{
    public class TreeIterator<TItem>
    {
        public TreeIterator(TreeTraversal order = TreeTraversal.PostOrder)
        {
            this.order = order;
        }

        private readonly TreeTraversal order;

        public IEnumerable<TItem> Flatten(TItem item) => Flatten(item, i => i as IEnumerable<TItem>);

        public IEnumerable<TItem> Flatten(TItem item, Func<TItem, IEnumerable<TItem>> stepInto)
        {
            if (this.order == TreeTraversal.PreOrder)
                yield return item;

            IEnumerable<TItem> flatten(IEnumerable<TItem> container) =>
                container.SelectMany(children => Flatten(children, stepInto));

            foreach (var child in stepInto(item).IfExists(flatten))
                yield return child;

            if (this.order == TreeTraversal.PostOrder)
                yield return item;
        }
    }
}
