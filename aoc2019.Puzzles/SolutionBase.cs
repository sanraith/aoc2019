namespace aoc2019.Puzzles
{
    public abstract class SolutionBase : ISolution
    {
        public abstract string Part1(string input);

        public virtual string Part2(string input) => null;
    }
}
