
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lxdn.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<TItem> Push<TItem>(this IList<TItem> items, TItem item)
        {
            items.Add(item);
            return items;
        }

        public static IList Push(this IList items, object item)
        {
            items.Add(item);
            return items;
        }

        public static IList<TItem> PushMany<TItem>(this IEnumerable<TItem> items, IEnumerable<TItem> items2)
        {
            return items2.Aggregate((IList<TItem>)items.ToList(), (destination, item) => destination.Push(item));
        }
    }
}
