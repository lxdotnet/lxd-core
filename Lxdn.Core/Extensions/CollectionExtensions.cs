
using System;
using System.Linq;
using System.Collections.Generic;

namespace Lxdn.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static List<TItem> Push<TItem>(this List<TItem> items, TItem item)
        {
            items.Add(item);
            return items;
        }

        public static List<TItem> PushMany<TItem>(this List<TItem> items, IEnumerable<TItem> items2)
        {
            return items2.Aggregate(items, (destination, item) => destination.Push(item));
        }

        public static Dictionary<TKey, TValue> Push<TKey, TValue>(this Dictionary<TKey, TValue> items, TKey key, TValue value)
        {
            items.ThrowIf(x => x.ContainsKey(key), x => new ArgumentException($"Already exists: {key}", nameof(key))).Add(key, value);
            return items;
        }
    }
}
