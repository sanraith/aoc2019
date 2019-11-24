using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2019.Puzzles
{
    public abstract class SolutionBase : ISolution
    {
        public event EventHandler<SolutionProgressEventArgs> ProgressUpdated;

        public int MillisecondsBetweenProgressUpdates { get; set; } = 200;

        public CancellationToken CancellationToken { get; set; }

        public abstract Task<string> Part1(string input);

        public virtual Task<string> Part2(string input) => throw new NotImplementedException();

        protected virtual SolutionProgress Progress { get; set; } = new SolutionProgress();

        /// <summary>
        /// Returns true if <see cref="UpdateProgressAsync"/> should be called to update the UI of the solution runner. This happens every couple of milliseconds.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsUpdateProgressNeeded() => Environment.TickCount >= myUpdateTick;

        /// <summary>
        /// Updates the UI of the solution runner with the current progress, and schedules the next update a couple of milliseconds in the future.
        /// </summary>
        protected Task UpdateProgressAsync()
        {
            myUpdateTick = Environment.TickCount + MillisecondsBetweenProgressUpdates;
            ProgressUpdated?.Invoke(this, new SolutionProgressEventArgs(Progress));
            return Task.Delay(1, CancellationToken);
        }

        /// <summary>
        /// A scheduled tick from <see cref="Environment.TickCount"/>, when a progress update should happen.
        /// </summary>
        private int myUpdateTick = Environment.TickCount;
    }
}
