using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc2019.Puzzles.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(TItem item, int index)> WithIndex<TItem>(this IEnumerable<TItem> sequence) => sequence.Select((x, i) => (x, i));

        public static void Deconstruct<TValue>(this IEnumerable<TValue> sequence, out TValue v1, out TValue v2)
        {
            var enumerator = sequence.GetEnumerator();
            v1 = GetNextWithException(enumerator);
            v2 = GetNextWithException(enumerator);
            if (enumerator.MoveNext()) { throw new IndexOutOfRangeException("Too many elements in sequence!"); }
        }

        public static void Deconstruct<TValue>(this IEnumerable<TValue> sequence, out TValue v1, out TValue v2, out TValue v3)
        {
            var enumerator = sequence.GetEnumerator();
            v1 = GetNextWithException(enumerator);
            v2 = GetNextWithException(enumerator);
            v3 = GetNextWithException(enumerator);
            if (enumerator.MoveNext()) { throw new IndexOutOfRangeException("Too many elements in sequence!"); }
        }

        private static TValue GetNextWithException<TValue>(IEnumerator<TValue> enumerator)
        {
            if (!enumerator.MoveNext())
            {
                throw new IndexOutOfRangeException("Not enough elements in sequence!");
            }

            return enumerator.Current;
        }
    }
}
