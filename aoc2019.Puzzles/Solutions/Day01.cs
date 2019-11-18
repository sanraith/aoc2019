using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("First puzzle")]
    public sealed class Day01 : SolutionBase
    {
        public override async Task<string> Part1(string input)
        {
            var sum = 0;
            var mainSw = Stopwatch.StartNew();
            var targetTickCount = Environment.TickCount; // TickCount is faster than Stopwatch
            for (int i = 0; i < 5000000; i++)
            {
                sum += (i % 2) * -1;
                if (Environment.TickCount >= targetTickCount)
                {
                    await Task.Delay(1, CancellationToken);
                    targetTickCount += 200;
                }
            }
            return mainSw.ElapsedMilliseconds.ToString();
        }

        //public override async Task<string> Part1(string input)
        //{
        //    var sum = 0;
        //    var mainSw = Stopwatch.StartNew();
        //    for (int i = 0; i < 5000000; i++)
        //    {
        //        sum += (i % 2) * -1;
        //        await HandleUserInputIfNeeded();
        //    }
        //    return mainSw.ElapsedMilliseconds.ToString();
        //}

        public override async Task<string> Part2(string input)
        {
            await Task.Delay(100);
            var sum = 0;
            var mainSw = Stopwatch.StartNew();
            for (int i = 0; i < 5000000; i++)
            {
                sum += (i % 2) * -1;
            }

            return mainSw.ElapsedMilliseconds.ToString();
        }
    }
}
