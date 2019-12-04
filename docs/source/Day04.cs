using aoc2019.Puzzles.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Secure Container")]
    public sealed class Day04 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var (min, max) = ParseRange(input);
            var count = await CountPasswords(min, max, hasExactlyTwoAdjacentDigits: false);

            return count.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var (min, max) = ParseRange(input);
            var count = await CountPasswords(min, max, hasExactlyTwoAdjacentDigits: true);

            return count.ToString();
        }

        private async Task<int> CountPasswords(int min, int max, bool hasExactlyTwoAdjacentDigits)
        {
            var count = 0;
            for (var number = min; number <= max; number++)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(number - min, max - min); }

                var digits = number.ToString();
                var neverDecreases = true;
                for (var i = 1; i < digits.Length; i++)
                {
                    if (digits[i] < digits[i - 1]) { neverDecreases = false; break; }
                }

                if (neverDecreases && digits.GroupBy(x => x).Any(x => hasExactlyTwoAdjacentDigits ? x.Count() == 2 : x.Count() > 1))
                {
                    count++;
                }
            }

            return count;
        }

        private static (int Min, int Max) ParseRange(string input)
        {
            var parts = GetLines(input).First().Split('-').Select(x => Convert.ToInt32(x)).ToList();
            return (parts[0], parts[1]);
        }
    }
}
