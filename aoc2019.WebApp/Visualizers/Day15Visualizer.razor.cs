using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using aoc2019.Puzzles.Solutions;
using aoc2019.WebApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day15;

namespace aoc2019.WebApp.Visualizers
{
    [TargetSolution(typeof(Day15))]
    public sealed partial class Day15Visualizer : ComponentBase, IVisualizer
    {
        [Parameter]
        public ISolution SolutionInstance { get; set; }

        [Inject]
        private IVisualizerHandler VisualizerHandler { get; set; }

        private string Content { get; set; }

        private bool IsPlaying { get; set; }

        protected override async Task OnParametersSetAsync() => await PlayAsync(1);

        private async Task PlayAsync(int frameCap = int.MaxValue)
        {
            IsPlaying = true;

            var cancellationToken = VisualizerHandler.GetVisualizationCancellationToken();
            var solution = (Day15)SolutionInstance;
            var originalMap = solution.Map;
            var topLeft = new Point(originalMap.Keys.Min(p => p.X), originalMap.Keys.Min(p => p.Y));
            var bottomRight = new Point(originalMap.Keys.Max(p => p.X), originalMap.Keys.Max(p => p.Y));
            var map = new Dictionary<Point, Tile>()
            {
                [topLeft] = Tile.Unknown,
                [bottomRight] = Tile.Unknown,
                [Point.Empty] = Tile.Robot
            };

            var index = 0;
            foreach (var (pos, tile) in originalMap)
            {
                if (index++ >= frameCap) { break; }
                if (index != 0) { map[pos] = tile; }

                if (index % 5 == 0 && !await RenderMap(map, topLeft, bottomRight, cancellationToken)) { break; }
            }

            foreach (var (pos, pathIndex) in solution.PathToOxygenGenerator.WithIndex())
            {
                if (index++ >= frameCap || pathIndex + 1 >= solution.PathToOxygenGenerator.Count) { break; }
                map[pos] = Tile.Empty;
                map[solution.PathToOxygenGenerator[pathIndex + 1]] = Tile.Robot;

                if (!await RenderMap(map, topLeft, bottomRight, cancellationToken, 5)) { break; }
            }

            foreach (var pos in solution.OxygenVisited)
            {
                if (index++ >= frameCap) { break; }
                map[pos] = Tile.OxygenSystem;

                if (index % 2 == 0 && !await RenderMap(map, topLeft, bottomRight, cancellationToken)) { break; }
            }

            await RenderMap(map, topLeft, bottomRight, cancellationToken, 0);
            IsPlaying = false;
        }

        private async Task<bool> RenderMap(Dictionary<Point, Tile> map, Point topLeft, Point bottomRight, CancellationToken cancellationToken, int delay = 10)
        {
            var sb = new StringBuilder();
            for (var y = topLeft.Y; y <= bottomRight.Y; y++)
            {
                for (var x = topLeft.X; x <= bottomRight.X; x++)
                {
                    var c = ' ';
                    if (map.TryGetValue(new Point(x, y), out var tile))
                    {
                        switch (tile)
                        {
                            case Tile.Empty: c = '.'; break;
                            case Tile.Wall: c = '#'; break;
                            case Tile.OxygenSystem: c = 'O'; break;
                            case Tile.Robot: c = 'X'; break;
                        }
                    }
                    sb.Append(c);
                }
                sb.AppendLine();
            }
            Content = sb.ToString();

            StateHasChanged();
            try
            {
                await Task.Delay(delay, cancellationToken);
                return true;
            }
            catch (TaskCanceledException) { return false; }
        }

        private void Stop()
        {
            IsPlaying = false;
            VisualizerHandler.CancelAllVisualizations();
        }
    }
}
