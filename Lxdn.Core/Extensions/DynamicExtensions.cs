using System;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Basics;

namespace Lxdn.Core.Extensions
{
    public static class CollectionsExtensions
    {
        public static CaseInsensitiveExpando ToDynamic(this IDictionary<string, object> obj)
        {
            return obj.Where(property => property.Value != null)
                .Aggregate(new CaseInsensitiveExpando(), (expando, property) =>
            {
                var nested = property.Value as IDictionary<string, object>;
                return expando.Set(property.Key, nested != null ? nested.ToDynamic() : property.Value);
            });
        }

        public static CaseInsensitiveExpando ToDynamic<TItem>(this IEnumerable<TItem> items,
                                                     Func<TItem, string> keySelector,
                                                     Func<TItem, object> elementSelector)
        {
            return items.ToDictionary(keySelector, elementSelector).ToDynamic();
        }
    }
}
