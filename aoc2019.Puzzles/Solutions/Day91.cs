using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("First test puzzle")]
    public sealed class Day91 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var sum = 0;
            var mainSw = Stopwatch.StartNew();
            for (int i = 0; i < Max; i++)
            {
                sum += (i % 2) * -1;
                if (IsUpdateProgressNeeded())
                {
                    Progress.Percentage = i / (double)Max * 100;
                    await UpdateProgressAsync();
                }
            }
            return mainSw.ElapsedMilliseconds.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            await UpdateProgressAsync();
            var sum = 0;
            var mainSw = Stopwatch.StartNew();
            for (int i = 0; i < Max; i++)
            {
                sum += (i % 2) * -1;
            }

            return mainSw.ElapsedMilliseconds.ToString();
        }

        private const int Max = 5000000;
    }
}
