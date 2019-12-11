using aoc2019.Puzzles.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntMachine = aoc2019.Puzzles.Solutions.Day09.IntMachine;
using Point = aoc2019.Puzzles.Solutions.Day10.Point;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Space Police")]
    public sealed class Day11 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new IntMachine(input);
            var canvas = new Dictionary<Point, int>();
            var paintedPositionCount = await Paint(intMachine, canvas);

            return paintedPositionCount.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var intMachine = new IntMachine(input);
            var canvas = new Dictionary<Point, int> { [new Point(0, 0)] = 1 };
            await Paint(intMachine, canvas);

            return Render(canvas);
        }

        private static string Render(Dictionary<Point, int> canvas)
        {
            var whitePoints = canvas.Where(x => x.Value == 1).Select(x => x.Key).ToList();
            var topLeft = new Point(whitePoints.Min(x => x.X), whitePoints.Min(x => x.Y));
            var bottomRight = new Point(whitePoints.Max(x => x.X), whitePoints.Max(x => x.Y));
            var resultSb = new StringBuilder();
            for (var x = topLeft.X; x <= bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y <= bottomRight.Y; y++)
                {
                    if (canvas.TryGetValue(new Point(x, y), out var color))
                    {
                        resultSb.Append(color == 0 ? ' ' : '#');
                    }
                    else
                    {
                        resultSb.Append(' ');
                    }
                }
                resultSb.AppendLine();
            }

            return resultSb.ToString();
        }

        private async Task<int> Paint(IntMachine intMachine, Dictionary<Point, int> canvas)
        {
            var paintedPositions = new HashSet<Point>();
            var directions = new[] { new Point(-1, 0), new Point(0, 1), new Point(1, 0), new Point(0, -1) };
            var direction = 0;
            var pos = new Point(0, 0);

            _ = intMachine.RunProgramAsync();
            while (true)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
                if (!canvas.TryGetValue(pos, out var color))
                {
                    color = 0;
                    canvas[pos] = color;
                }

                await intMachine.InputChannel.WriteAsync(color);
                if (!await intMachine.OutputChannel.WaitToReadAsync())
                {
                    break;
                }

                canvas[pos] = (int)await intMachine.OutputChannel.ReadAsync();
                paintedPositions.Add(pos);
                var directionDelta = (int)await intMachine.OutputChannel.ReadAsync() == 0 ? -1 : 1;
                direction = (direction + directionDelta + 4) % 4;
                pos += directions[direction];
            }

            return paintedPositions.Count;
        }
    }
}
