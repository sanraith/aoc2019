using System.Collections.Generic;

namespace aoc2019.Puzzles.Core
{
    public interface ISolutionHandler
    {
        IReadOnlyDictionary<int, SolutionMetadata> Solutions { get; }
    }
}
