using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
            var inputSequence = Enumerable.Repeat(GetSequence(input), 10000).SelectMany(x => x).ToArray();
            var offset = Convert.ToInt32(string.Join(string.Empty, inputSequence.Take(7)), 10);

            // This less-than-optimal solution provides an answer under 7 hours on an Intel Core i7-3770 @ 3.4 GHz.
            // I may add a more optimal solution later.
            // Toggle comment on the following lines if you are feeling adventurous.

            throw new Exception(@"Super slow solution. Try it at your own risk.");
            //var resultSequence = await RunFFT2(inputSequence, 100, 10000, offset: offset);
            //return string.Join(string.Empty, resultSequence.Skip(offset).Take(8));
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

        private async Task<int[]> RunFFT2(int[] sequence, int phaseCount, int sequenceRepeatCount, int offset)
        {
            myLcmCache.Clear();
            var pattern = new[] { 0, 1, 0, -1 };
            var patternLength = pattern.Length;
            var sequenceLength = sequence.Length;
            var sequencePeriodLength = sequence.Length / sequenceRepeatCount;

            var sw = Stopwatch.StartNew();

            var nextSequence = new int[sequenceLength];
            for (var phaseIndex = 0; phaseIndex < phaseCount; phaseIndex++)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(phaseIndex, phaseCount); }
                
                CalculatePhase(sequence, offset, pattern, patternLength, sequenceLength, sequencePeriodLength, nextSequence);
                (sequence, nextSequence) = (nextSequence, sequence);
                Console.WriteLine($"Current phase: {phaseIndex} {sw.Elapsed}");
            }
            Console.WriteLine(sw.Elapsed);

            return sequence;
        }

        private static void CalculatePhase(int[] sequence, int offset, int[] pattern, int patternLength, int sequenceLength, int sequencePeriodLength, int[] nextSequence)
        {
            Parallel.For(offset, sequenceLength, outputIndex =>
            {
                int sum = Sum(sequence, offset, pattern, patternLength, sequenceLength, sequencePeriodLength, outputIndex);
                nextSequence[outputIndex] = Math.Abs(sum) % 10;
            });
        }

        private static int Sum(int[] sequence, int offset, int[] pattern, int patternLength, int sequenceLength, int sequencePeriodLength, int outputIndex)
        {
            var sum = 0;
            var relevantSize = outputIndex + 1;
            var startIndex = relevantSize - 1;
            var index = startIndex;

            // reach a period
            while (index % sequencePeriodLength != 0)
            {
                sum += sequence[index];
                index++;
            }
            var span = sequence.AsSpan();

            // sum period
            var remainingPeriodCount = (sequenceLength - index) / sequencePeriodLength;
            var maxPeriodIndex = index + remainingPeriodCount * sequencePeriodLength;
            if (remainingPeriodCount > 0)
            {
                var prevPeriodSum = -1;
                while (index < maxPeriodIndex)
                {
                    bool allZero = false;
                    var periodSum = 0;
                    do
                    {
                        var patternItem = pattern[(index + 1) / (outputIndex + 1) % patternLength];
                        periodSum += sequence[index] * patternItem;
                        allZero |= sequence[index] == 0;
                        index++;
                    } while (index % sequencePeriodLength != 0);

                    var original = span.Slice(index - sequencePeriodLength * 2, sequencePeriodLength);
                    if (!allZero && periodSum == prevPeriodSum && original.SequenceEqual(span.Slice(index - sequencePeriodLength, sequencePeriodLength)))
                    {
                        var equalCount = 0;
                        for (var sliceIndex = index - sequencePeriodLength; sliceIndex < sequenceLength; sliceIndex += sequencePeriodLength)
                        {
                            var other = span.Slice(sliceIndex, sequencePeriodLength);
                            if (!original.SequenceEqual(other))
                            {
                                break;
                            }
                            equalCount++;
                        }
                        if (equalCount != remainingPeriodCount)
                        {
                            sum += periodSum;
                            break;
                        }
                        sum += equalCount * periodSum;
                        index += (equalCount - 1) * sequencePeriodLength;
                        break;
                    }
                    else
                    {
                        sum += periodSum;
                        prevPeriodSum = periodSum;
                        remainingPeriodCount--;
                    }
                }
            }

            // add the remaining
            while (index < sequenceLength)
            {
                var patternItem = pattern[(index + 1) / (outputIndex + 1) % patternLength];
                sum += sequence[index] * patternItem;
                index++;
            }

            return sum;
        }

        private Dictionary<(int, int), int> myLcmCache = new Dictionary<(int, int), int>();

        private static int[] GetSequence(string input) => GetLines(input).First().Select(c => c - 48).ToArray();
    }
}
