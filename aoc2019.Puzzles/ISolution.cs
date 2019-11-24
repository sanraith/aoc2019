using System;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2019.Puzzles
{
    public interface ISolution
    {
        event EventHandler<SolutionProgressEventArgs> ProgressUpdated;

        CancellationToken CancellationToken { get; set; }

        Task<string> Part1(string input);

        Task<string> Part2(string input);
    }
}
