using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("First puzzle")]
    public sealed class Day01 : SolutionBase
    {
        public override async Task<string> Part1(string input)
        {
            await Task.Delay(1000, CancellationToken);
            return input;
        }

        public override async Task<string> Part2(string input)
        {
            await Task.Delay(1000, CancellationToken);
            return new string(input.Reverse().ToArray());
        }
    }
}
