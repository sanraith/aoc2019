using aoc2019.Puzzles.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day11;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Oxygen System")]
    public sealed class Day15 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            await DiscoverMap(input);

            return myPathToOxygenGenerator.Count.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            await DiscoverMap(input);

            var oxygenGeneratorPos = myMap.First(x => x.Value == Tile.OxygenSystem).Key;
            var maxDistance = 0;
            var visited = new HashSet<Point>();
            var queue = new Queue<(Point p, int distance)>(new[] { (oxygenGeneratorPos, 0) });
            while (queue.Count > 0)
            {
                var (pos, distance) = queue.Dequeue();
                if (!visited.Add(pos)) { continue; }
                if (distance > maxDistance) { maxDistance = distance; };

                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                for (var directionCode = 1; directionCode <= 4; directionCode++)
                {
                    var direction = myDirections[directionCode];
                    var nextPos = pos + direction;
                    if (visited.Contains(nextPos) || myMap[nextPos] == Tile.Wall) { continue; }

                    queue.Enqueue((nextPos, distance + 1));
                }
            }

            return maxDistance.ToString();
        }

        public string RenderMap()
        {
            var topLeft = new Point(myMap.Keys.Min(p => p.X), myMap.Keys.Min(p => p.Y));
            var bottomRight = new Point(myMap.Keys.Max(p => p.X), myMap.Keys.Max(p => p.Y));

            var sb = new StringBuilder();
            for (var y = topLeft.Y; y <= bottomRight.Y; y++)
            {
                for (var x = topLeft.X; x <= bottomRight.X; x++)
                {
                    var c = ' ';
                    if (myMap.TryGetValue(new Point(x, y), out var tile))
                    {
                        switch (tile)
                        {
                            case Tile.Empty: c = '.'; break;
                            case Tile.Wall: c = '#'; break;
                            case Tile.OxygenSystem: c = 'O'; break;
                        }
                    }
                    if (x == 0 && y == 0) { c = 'X'; }
                    sb.Append(c);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private async Task DiscoverMap(string input)
        {
            myIntMachine = new SynchronousIntMachine(input);
            myMap = new Dictionary<Point, Tile>() { [new Point(0, 0)] = Tile.Empty };
            myPathToOxygenGenerator = new List<Point>();
            await Backtrack(new Point(0, 0), null);
        }

        private async Task<bool> Backtrack(Point pos, Point? backDirection)
        {
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

            var backDirectionCode = -1;
            var foundPath = false;
            for (var directionCode = 1; directionCode <= 4; directionCode++)
            {
                var direction = myDirections[directionCode];
                if (direction == backDirection) { backDirectionCode = directionCode; continue; }

                var nextPos = pos + direction;
                if (myMap.ContainsKey(nextPos)) { continue; }

                long tileCode = Step(directionCode);
                switch (tileCode)
                {
                    case 0:
                        myMap[nextPos] = Tile.Wall;
                        break;
                    case 1:
                        myMap[nextPos] = Tile.Empty;
                        if (await Backtrack(nextPos, direction * -1))
                        {
                            myPathToOxygenGenerator.Add(pos);
                            foundPath = true;
                        }
                        break;
                    case 2:
                        myMap[nextPos] = Tile.OxygenSystem;
                        myPathToOxygenGenerator.Add(pos);
                        foundPath = true;
                        await Backtrack(nextPos, direction * -1);
                        break;
                }
            }

            if (backDirection.HasValue)
            {
                Step(backDirectionCode);
            }

            return foundPath;
        }

        private long Step(int directionCode)
        {
            myIntMachine.InputQueue.Enqueue(directionCode);
            myIntMachine.RunUntilBlockOrComplete();
            var tileCode = myIntMachine.OutputQueue.Dequeue();

            return tileCode;
        }

        private SynchronousIntMachine myIntMachine;
        private Dictionary<Point, Tile> myMap;
        private List<Point> myPathToOxygenGenerator;

        private readonly Dictionary<int, Point> myDirections = new Dictionary<int, Point>
        {
            [1] = new Point(0, -1), // North
            [2] = new Point(0, 1),  // South
            [3] = new Point(-1, 0), // West
            [4] = new Point(1, 0)   // East
        };

        private enum Tile { Empty, Wall, OxygenSystem }
    }
}
