using System.Linq;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("First puzzle")]
    public sealed class Day01 : SolutionBase
    {
        public override string Part1(string input)
        {
            return input;
        }

        public override string Part2(string input)
        {
            return new string(input.Reverse().ToArray());
        }
    }
}
