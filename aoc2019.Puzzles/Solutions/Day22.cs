using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Slam Shuffle")]
    public sealed class Day22 : SolutionBase
    {
        public int CardCount { get; set; } = 10007;

        public int[] LastStack { get; private set; }

        public override async Task<string> Part1Async(string input)
        {
            var steps = ParseSteps(input);
            var stack = Enumerable.Range(0, CardCount).ToArray();
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
            LastStack = stack;

            int result = stack.Length > 2019 ? Array.IndexOf(stack, 2019) : 0;
            return result.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
            throw new NotImplementedException();
        }

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
