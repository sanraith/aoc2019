using System;
using System.Collections.Generic;

namespace aoc2019.Puzzles.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueGenerator)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueGenerator(key);
                dictionary.Add(key, value);
            }

            return value;
        }
    }
}
