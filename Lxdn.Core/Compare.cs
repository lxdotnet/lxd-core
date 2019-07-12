
using System;
using System.Collections.Generic;

using Lxdn.Core.Extensions;

namespace Lxdn.Core
{
    public class Compare<TItem> : IEqualityComparer<TItem>
    {
        private readonly Func<TItem, object>[] properties;

        private Compare(params Func<TItem, object>[] properties)
        {
            this.properties = properties;
        }

        public static Compare<TItem> Using(params Func<TItem, object>[] properties) => new Compare<TItem>(properties);

        public bool Equals(TItem x, TItem y)
        {
            if (x != null && y != null)
            {
                return x.HashUsing(properties) == y.HashUsing(properties);
            }

            return x == null && y == null;
        }

        public int GetHashCode(TItem item) => item.HashUsing(properties);
    }
}
