using System.Collections.Generic;

namespace aoc2019.Puzzles
{
    public interface ISolutionHandler
    {
        IReadOnlyDictionary<int, SolutionMetadata> Solutions { get; }
    }
}
