
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

        public static Dictionary<TKey, TValue> Push<TKey, TValue>(this Dictionary<TKey, TValue> items, TKey key, TValue value, Action onConflict)
        {
            if (!items.ContainsKey(key))
                items.Add(key, value);
            else onConflict();

            return items;
        }

        public static Dictionary<TKey, TValue> Push<TKey, TValue>(this Dictionary<TKey, TValue> items, TKey key, TValue value)
            => items.Push(key, value, () => throw new ArgumentException($"Already exists: {key}", nameof(key)));

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.ContainsKey(key) ? dictionary[key] : default;
    }
}
