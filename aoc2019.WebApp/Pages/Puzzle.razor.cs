using aoc2019.Puzzles;
using aoc2019.WebApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Pages
{
    public sealed partial class Puzzle : ComponentBase
    {
        [Parameter]
        public string Day { get; set; }

        [Parameter]
        public int MillisBetweenProgressRender { get; set; } = 500;

        [Inject]
        private ISolutionHandler SolutionHandler { get; set; }

        [Inject]
        private IInputHandler InputHandler { get; set; }

        private SolutionMetadata SolutionMetadata { get; set; }
        private string Input { get; set; }
        private object[] Results { get; set; }
        private bool IsWorking { get; set; }
        private SolutionProgress Progress { get; set; }

        protected override Task OnParametersSetAsync() => Init();

        private async Task Init()
        {
            Cancel();
            SolutionMetadata = null;
            Input = null;
            Results = null;
            Progress = new SolutionProgress();
            if (int.TryParse(Day, out var dayNumber) && SolutionHandler.Solutions.TryGetValue(dayNumber, out var solutionMetadata))
            {
                SolutionMetadata = solutionMetadata;
                Input = await InputHandler.GetInputAsync(SolutionMetadata.Day);
                Results = InputHandler.GetResults(SolutionMetadata.Day);
            }
        }

        private void Cancel() => myCancellationTokenSource?.Cancel(true);

        private async Task SolveAsync()
        {
            myCancellationTokenSource = new CancellationTokenSource();
            ISolution solution = null;
            try
            {
                IsWorking = true;
                InputHandler.ClearResults(SolutionMetadata.Day);
                solution = SolutionMetadata.CreateInstance();
                solution.CancellationToken = myCancellationTokenSource.Token;
                solution.ProgressUpdated += OnProgressUpdate;

                foreach (var (part, index) in new Func<string, Task<string>>[] { solution.Part1, solution.Part2 }.Select((x, i) => (x, i)))
                {
                    Progress = new SolutionProgress();
                    StateHasChanged();
                    await Task.Delay(1);
                    Results[index] = await ExceptionToResult(part);
                }
            }
            finally
            {
                if (solution != null) { solution.ProgressUpdated -= OnProgressUpdate; }
                IsWorking = false;
            }
        }

        private void OnProgressUpdate(object sender, SolutionProgressEventArgs args)
        {
            Progress = args.Progress;
            if (Environment.TickCount > myProgressRenderTick)
            {
                StateHasChanged();
                myProgressRenderTick = Environment.TickCount + MillisBetweenProgressRender;
            }
        }

        private async Task<object> ExceptionToResult(Func<string, Task<string>> func)
        {
            try
            {
                return await (func(Input) ?? Task.FromResult<string>(null));
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        private CancellationTokenSource myCancellationTokenSource;
        private int myProgressRenderTick = Environment.TickCount;
    }
}
