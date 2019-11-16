using System.Threading.Tasks;

namespace aoc2019.Puzzles
{
    public interface ISolution
    {
        Task<string> Part1(string input);

        Task<string> Part2(string input);
    }
}
