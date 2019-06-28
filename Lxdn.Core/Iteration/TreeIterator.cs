
using System;
using System.Collections.Generic;

namespace Lxdn.Core
{
    public class TreeIterator<TItem>
    {
        public TreeIterator(TreeTraversal order)
        {
            this.order = order;
        }

        public TreeIterator() : this(TreeTraversal.PostOrder) {}

        private readonly TreeTraversal order;

        public IEnumerable<TItem> Flatten(TItem item)
        {
            return this.Flatten(item, i => i as IEnumerable<TItem>);
        }

        public IEnumerable<TItem> Flatten(TItem item, Func<TItem, IEnumerable<TItem>> enumerator)
        {
            if (this.order == TreeTraversal.PreOrder)
                yield return item;

            var container = enumerator(item);
            if (container != null)
            {
                foreach (TItem child in container)
                {
                    foreach (TItem flattenedChild in this.Flatten(child, enumerator))
                        yield return flattenedChild;
                } 
            }

            if (this.order == TreeTraversal.PostOrder)
                yield return item;
        }
    }
}
