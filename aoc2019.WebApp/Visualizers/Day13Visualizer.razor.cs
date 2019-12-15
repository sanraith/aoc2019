using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using aoc2019.Puzzles.Solutions;
using aoc2019.WebApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Visualizers
{
    [TargetSolution(typeof(Day13))]
    public sealed partial class Day13Visualizer : ComponentBase, IVisualizer
    {
        [Parameter]
        public ISolution SolutionInstance { get; set; }

        [Inject]
        private IVisualizerHandler VisualizerHandler { get; set; }

        private string Content { get; set; }

        private int Score { get; set; }

        private bool IsPlaying { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Score = 0;
            await PlayAsync(1);
        }

        private async Task PlayAsync(int frameCap = int.MaxValue)
        {
            IsPlaying = true;

            var cancellationToken = VisualizerHandler.GetVisualizationCancellationToken();
            List<List<(Day10.Point Pos, long Tile)>> frames = ((Day13)SolutionInstance).VisualizationFrames;
            char[][] tiles = null;
            foreach (var (frame, index) in frames.WithIndex())
            {
                if (index >= frameCap) { break; }

                if (tiles == null)
                {
                    var width = frame.Max(x => x.Pos.X) + 1;
                    var height = frame.Max(x => x.Pos.Y) + 1;
                    tiles = Enumerable.Range(0, height).Select(x => new char[width]).ToArray();
                }

                foreach (var (pos, tile) in frame)
                {
                    if (pos.X < 0)
                    {
                        Score = (int)tile;
                        continue;
                    }

                    char c;
                    switch ((Day13.Tile)tile)
                    {
                        case Day13.Tile.Empty: c = ' '; break;
                        case Day13.Tile.Wall: c = '▓'; break;
                        case Day13.Tile.Block: c = '■'; break;
                        case Day13.Tile.Paddle: c = '─'; break;
                        case Day13.Tile.Ball: c = 'o'; break;
                        default: c = '?'; break;
                    }
                    tiles[pos.Y][pos.X] = c;
                }

                Content = string.Join(Environment.NewLine, tiles.Select(line => new string(line)));
                StateHasChanged();

                try { await Task.Delay(10, cancellationToken); }
                catch (TaskCanceledException) { break; }
            }

            IsPlaying = false;
        }

        private void Stop()
        {
            IsPlaying = false;
            VisualizerHandler.CancelAllVisualizations();
        }
    }
}
