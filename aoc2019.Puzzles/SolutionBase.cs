using System.Threading.Tasks;

namespace aoc2019.Puzzles
{
    public abstract class SolutionBase : ISolution
    {
        public abstract Task<string> Part1(string input);

        public virtual Task<string> Part2(string input) => null;
    }
}
