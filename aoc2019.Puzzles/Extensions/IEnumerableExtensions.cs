using System.Collections.Generic;
using System.Linq;

namespace aoc2019.Puzzles.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(TItem item, int index)> WithIndex<TItem>(this IEnumerable<TItem> sequence) => sequence.Select((x, i) => (x, i));
    }
}
