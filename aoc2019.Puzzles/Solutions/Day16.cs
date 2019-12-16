using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Flawed Frequency Transmission")]
    public sealed class Day16 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var inputSequence = GetSequence(input);
            var resultSequence = await RunFFT(inputSequence, 100);

            return string.Join(string.Empty, resultSequence.Take(8));
        }

        public override async Task<string> Part2Async(string input)
        {
            var inputSequence = Enumerable.Repeat(GetSequence(input), 10000).SelectMany(x => x).ToArray();
            var offset = Convert.ToInt32(string.Join(string.Empty, inputSequence.Take(7)), 10);
            
            throw new NotImplementedException();
            
            var resultSequence = await RunFFT(inputSequence, 100);
            return string.Join(string.Empty, resultSequence.Skip(offset).Take(8));
        }

        private async Task<int[]> RunFFT(int[] sequence, int phaseCount)
        {
            var pattern = new[] { 0, 1, 0, -1 };
            var patternLength = pattern.Length;
            var sequenceLength = sequence.Length;

            var nextSequence = new int[sequenceLength];
            for (var phaseIndex = 0; phaseIndex < phaseCount; phaseIndex++)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(phaseIndex, phaseCount); }

                for (var outputIndex = 0; outputIndex < sequenceLength; outputIndex++)
                {
                    var sum = 0;
                    for (var i = 0; i < sequenceLength; i++)
                    {
                        sum += sequence[i] * pattern[(i + 1) / (outputIndex + 1) % patternLength];
                    }
                    nextSequence[outputIndex] = Math.Abs(sum) % 10;
                }
                (sequence, nextSequence) = (nextSequence, sequence);
            }

            return sequence;
        }

        private static int[] GetSequence(string input) => GetLines(input).First().Select(c => c - 48).ToArray();
    }
}
