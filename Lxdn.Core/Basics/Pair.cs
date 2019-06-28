using System;
using System.Collections.Generic;

namespace Lxdn.Core.Basics
{
    public class Pair<TItem>
    {
        public Pair(TItem left, TItem right)
        {
            this.Left = left;
            this.Right = right;
        }

        public Pair(IList<TItem> items)
        {
            if (items.Count != 2)
                throw new ArgumentOutOfRangeException("items", "The count of items must be 2");

            this.Left = items[0];
            this.Right = items[1];
        }

        public Pair<TOther> ConvertTo<TOther>(Func<TItem, TOther> converter)
        {
            return new Pair<TOther>(converter(this.Left), converter(this.Right));
        }

        public TItem Left { get; private set; }

        public TItem Right { get; private set; }
    }
}
