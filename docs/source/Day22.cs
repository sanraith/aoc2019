using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Slam Shuffle")]
    public sealed class Day22 : SolutionBase
    {
        public int Part1CardCount { get; set; } = 10007;

        public int[] Part1LastStack { get; private set; }

        public override async Task<string> Part1Async(string input)
        {
            var steps = ParseSteps(input);
            var stack = Enumerable.Range(0, Part1CardCount).ToArray();
            var stackLength = stack.Length;
            var newStack = new int[stackLength];

            var stackMemory = stack.AsMemory();
            var newStackMemory = newStack.AsMemory();

            foreach (var ((technique, tParam), index) in steps.WithIndex())
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(index, steps.Count); }

                switch (technique)
                {
                    case Technique.Cut:
                        var positiveOffset = tParam;
                        if (positiveOffset < 0) { positiveOffset += stackLength * (Math.Abs(positiveOffset) / stackLength + 1); }
                        positiveOffset %= stackLength;
                        stackMemory.Slice(0, positiveOffset).CopyTo(newStackMemory.Slice(stackLength - positiveOffset));
                        stackMemory.Slice(positiveOffset).CopyTo(newStackMemory);
                        break;
                    case Technique.DealWithIncrement:
                        var originalPos = 0;
                        var newPos = 0;
                        while (originalPos < stackLength)
                        {
                            newStack[newPos] = stack[originalPos];
                            originalPos++;
                            newPos = (newPos + tParam) % stackLength;
                        }
                        break;
                    case Technique.DealIntoNewStack:
                        for (var i = 0; i < stackLength; i++)
                        {
                            newStack[i] = stack[stackLength - i - 1];
                        }
                        break;
                }
                (stack, newStack) = (newStack, stack);
                (stackMemory, newStackMemory) = (newStackMemory, stackMemory);
            }
            Part1LastStack = stack;

            int result = stack.Length > 2019 ? Array.IndexOf(stack, 2019) : 0;
            return result.ToString();
        }

        public override string Part2(string input)
        {
            const long stackLength = 119315717514047;
            const long iterationCount = 101741582076661;
            const long targetPos = 2020;
            var steps = ParseSteps(input);

            // nextPos = (a * pos + b) % stackLength;
            BigInteger a = 1;
            BigInteger b = 0;
            foreach (var (technique, tParam) in steps)
            {
                switch (technique)
                {
                    case Technique.Cut:
                        b = stackLength + b - tParam;
                        break;
                    case Technique.DealWithIncrement:
                        a *= tParam;
                        b *= tParam;
                        break;
                    case Technique.DealIntoNewStack:
                        a *= -1;
                        b = stackLength - b - 1;
                        break;
                }
            }

            // Represent the gazillion steps with a single step
            var aGazillion = BigInteger.ModPow(a, iterationCount, stackLength);
            var bGazillion = b * (BigInteger.ModPow(a, iterationCount, stackLength) - 1) * ModuloInverse(a - 1, stackLength) % stackLength;

            // nextPos = (a * pos + b) % stackLength;
            // x = a * pos
            // nextPos = (x + b) % stackLength;
            // x = (nextPos - b) % stackLength
            // pos = ((nextPos - b) % stackLength) / a
            //
            // But division does not work in modulo arithmetic, so...
            //
            // pos = (((nextPos - b) % stackLength) * ModInv(a, stackLength)) % stackLength
            var result = (((targetPos - bGazillion) % stackLength) * ModuloInverse(aGazillion, stackLength)) % stackLength;

            return result.ToString();
        }

        private static BigInteger ModuloInverse(BigInteger a, BigInteger n) => BigInteger.ModPow(a, n - 2, n);

        private List<(Technique Technique, int Param)> ParseSteps(string input)
        {
            var numberRegex = new Regex("[-0-9]+");
            var steps = new List<(Technique Technique, int Param)>();
            foreach (var line in GetLines(input))
            {
                var match = numberRegex.Match(line);
                var number = match.Success ? Convert.ToInt32(match.Value) : 0;
                Technique technique;
                switch (line)
                {
                    case var _ when line.Contains("cut"): technique = Technique.Cut; break;
                    case var _ when line.Contains("deal into new stack"): technique = Technique.DealIntoNewStack; break;
                    case var _ when line.Contains("deal with increment"): technique = Technique.DealWithIncrement; break;
                    default: throw new InvalidOperationException("Unknown technique: " + line);
                }
                steps.Add((technique, number));
            }

            return steps;
        }

        private enum Technique { Cut, DealWithIncrement, DealIntoNewStack };
    }
}
