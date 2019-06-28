
using System.Collections.Generic;
using System.Linq;

namespace Lxdn.Core.Extensions
{
    public static class Combinatorics
    {
        // https://stackoverflow.com/questions/5980810/generate-all-unique-combinations-of-elements-of-a-ienumerableof-t

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    // Exclude items that were already picked
                    from item in sequence.Except(accseq)
                    // Enforce ascending order to avoid same sequence in different order
                    where !accseq.Any() || Comparer<T>.Default.Compare(item, accseq.Last()) > 0
                    select accseq.Concat(new[] { item }).ToList()).ToArray();
        }

        public static IEnumerable<IEnumerable<TItem>> Combinations<TItem>(this IEnumerable<TItem> items)
        {
            var empty = Enumerable.Empty<IEnumerable<TItem>>();

            return Enumerable.Range(1, items.Count())
                .Aggregate(empty, (combinations, i) => combinations.Concat(Enumerable.Repeat(items, i).Combinations()).ToList())
                .ToList();
        }
    }
}
