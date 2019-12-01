using System;

namespace aoc2019.Puzzles.Core
{
    public sealed class SolutionProgressEventArgs : EventArgs
    {
        public SolutionProgress Progress;

        public SolutionProgressEventArgs(SolutionProgress solutionProgress)
        {
            Progress = solutionProgress;
        }
    }
}
