using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day11;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Set and Forget")]
    public sealed class Day17 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);

            var lines = new List<char[]>();
            var line = new List<char>();
            while (intMachine.RunUntilBlockOrComplete() != SynchronousIntMachine.ReturnCode.Completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                var c = intMachine.OutputQueue.Dequeue();
                if (c == 10 && line.Any())
                {
                    lines.Add(line.ToArray());
                    line = new List<char>();
                }
                else
                {
                    line.Add((char)c);
                }
            }

            var width = lines.First().Length;
            var height = lines.Count;
            var alignmentSum = 0;
            for (var y = 1; y < height - 1; y++)
            {
                for (var x = 1; x < width - 1; x++)
                {
                    if (lines[y][x] == '#' &&
                        lines[y - 1][x] == '#' &&
                        lines[y + 1][x] == '#' &&
                        lines[y][x + 1] == '#' &&
                        lines[y][x - 1] == '#')
                    {
                        alignmentSum += x * y;
                    }
                }
            }

            // Draw the map
            Console.WriteLine();
            lines.ForEach(l => Console.WriteLine(new string(l)));

            return alignmentSum.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
            throw new NotImplementedException();
        }
    }
}
