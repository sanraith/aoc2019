using aoc2019.Puzzles.Core;
using System;
using System.Linq;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Secure Container")]
    public sealed class Day04 : SolutionBase
    {
        public override string Part1(string input)
        {
            var (min, max) = ParseRange(input);
            var count = CountPasswords(min, max, hasExactlyTwoAdjacentDigits: false);

            return count.ToString();
        }

        public override string Part2(string input)
        {
            var (min, max) = ParseRange(input);
            var count = CountPasswords(min, max, hasExactlyTwoAdjacentDigits: true);

            return count.ToString();
        }

        private static int CountPasswords(int min, int max, bool hasExactlyTwoAdjacentDigits)
        {
            var digits = min.ToString().Select(x => Convert.ToInt32(x.ToString())).ToArray();
            return CountPasswordsRecursive(digits, max, 0, hasExactlyTwoAdjacentDigits);
        }

        private static int CountPasswordsRecursive(int[] digits, int target, int index, bool hasExactlyTwoAdjacentDigits)
        {
            var count = 0;
            var start = digits[index];
            var isLastDigit = index == digits.Length - 1;
            for (var digit = start; digit < 10; digit++)
            {
                digits[index] = digit;
                if (index != 0 && digits[index] < digits[index - 1])
                {
                    if (!isLastDigit) { digits[index + 1] = Math.Min(digits[index + 1], digit); }
                    continue;
                }
                if (IsNumberGreaterThanTarget(digits, target)) { break; }

                if (isLastDigit)
                {
                    var digitCount = new int[10];
                    foreach (var d in digits) { digitCount[d]++; }
                    if (digitCount.Any(x => hasExactlyTwoAdjacentDigits ? x == 2 : x > 1))
                    {
                        count++;
                    }
                }
                else
                {
                    count += CountPasswordsRecursive(digits, target, index + 1, hasExactlyTwoAdjacentDigits);
                    digits[index + 1] = digit;
                }
            }

            return count;
        }

        private static bool IsNumberGreaterThanTarget(int[] digits, int target)
        {
            var number = 0;
            var length = digits.Length;
            for (var i = 0; i < length; i++)
            {
                number += digits[length - i - 1] * (int)Math.Pow(10, i);
            }
            return number > target;
        }

        private static (int Min, int Max) ParseRange(string input)
        {
            var parts = GetLines(input).First().Split('-').Select(x => Convert.ToInt32(x)).ToList();
            return (parts[0], parts[1]);
        }
    }
}
