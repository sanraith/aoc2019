using System;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2019.Puzzles
{
    public abstract class SolutionBase : ISolution
    {
        public event EventHandler<EventArgs> ProgressUpdated;

        public CancellationToken CancellationToken { get; set; }

        protected void UpdateProgress()
        {
            ProgressUpdated?.Invoke(this, null);
        }

        protected Task HandleUserInputIfNeeded()
        {
            if (Environment.TickCount >= myTargetTickCount)
            {
                myTargetTickCount += 200;
                return Task.Delay(1, CancellationToken);
            }
            return CompletedTask;
        }

        public abstract Task<string> Part1(string input);

        public virtual Task<string> Part2(string input) => throw new NotImplementedException();
        
        private int myTargetTickCount = Environment.TickCount;

        private static readonly Task CompletedTask = Task.FromResult(true);
    }
}
