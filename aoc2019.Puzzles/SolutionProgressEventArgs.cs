using System;

namespace aoc2019.Puzzles
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
