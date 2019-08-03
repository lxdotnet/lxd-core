
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lxdn.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static List<TItem> Push<TItem>(this List<TItem> items, TItem item)
        {
            items.Add(item);
            return items;
        }

        //public static IList Push(this IList items, object item)
        //{
        //    items.Add(item);
        //    return items;
        //}

        public static List<TItem> PushMany<TItem>(this List<TItem> items, IEnumerable<TItem> items2)
        {
            return items2.Aggregate(items, (destination, item) => destination.Push(item));
        }
    }
}
