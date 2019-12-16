using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day11;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Oxygen System")]
    public sealed class Day15 : SolutionBase
    {
        public enum Tile { Robot, Empty, Wall, OxygenSystem, Unknown }

        public Dictionary<Point, Tile> Map { get; private set; }
        public HashSet<Point> OxygenVisited { get; private set; }
        public List<Point> PathToOxygenGenerator { get; private set; }

        public Day15() => myDirectionCodes = myDirections.ToDictionary(k => k.Value, v => v.Key);

        public override async Task<string> Part1Async(string input)
        {
            await DiscoverMap(input);

            return PathToOxygenGenerator.Count.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            await DiscoverMap(input);

            var oxygenGeneratorPos = Map.First(x => x.Value == Tile.OxygenSystem).Key;
            var maxDistance = 0;
            OxygenVisited = new HashSet<Point>();
            var queue = new Queue<(Point p, int distance)>(new[] { (oxygenGeneratorPos, 0) });
            while (queue.Count > 0)
            {
                var (pos, distance) = queue.Dequeue();
                if (!OxygenVisited.Add(pos)) { continue; }
                if (distance > maxDistance) { maxDistance = distance; };

                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                for (var directionCode = 1; directionCode <= 4; directionCode++)
                {
                    var direction = myDirections[directionCode];
                    var nextPos = pos + direction;
                    if (OxygenVisited.Contains(nextPos) || Map[nextPos] == Tile.Wall) { continue; }

                    queue.Enqueue((nextPos, distance + 1));
                }
            }

            return maxDistance.ToString();
        }

        private async Task DiscoverMap(string input)
        {
            myIntMachine = new SynchronousIntMachine(input);
            Map = new Dictionary<Point, Tile>() { [new Point(0, 0)] = Tile.Empty };
            await Backtrack();
            PathToOxygenGenerator = FindPath(Point.Empty, Map.First(x => x.Value == Tile.OxygenSystem).Key).Skip(1).ToList();
        }

        private async Task Backtrack()
        {
            var currentPos = Point.Empty;
            var stack = new Stack<(Point, int)>(myDirections.Keys.Select(c => (currentPos, c)));
            while (stack.Count > 0)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(Map.Count, Math.Max(Map.Count + 1, 41 * 41)); }

                var (pos, directionCode) = stack.Pop();
                var direction = myDirections[directionCode];
                var nextPos = pos + direction;
                if (Map.ContainsKey(nextPos)) { continue; }

                if (currentPos != pos)
                {
                    currentPos = GoTo(currentPos, pos);
                }

                long tileCode = Step(directionCode);
                switch (tileCode)
                {
                    case 0:
                        Map[nextPos] = Tile.Wall;
                        continue;
                    case 1:
                        Map[nextPos] = Tile.Empty;
                        break;
                    case 2:
                        Map[nextPos] = Tile.OxygenSystem;
                        break;
                }

                currentPos = nextPos;
                for (var nextDirectionCode = 1; nextDirectionCode <= 4; nextDirectionCode++)
                {
                    stack.Push((nextPos, nextDirectionCode));
                }
            }
        }

        private Point GoTo(Point currentPos, Point targetPos)
        {
            var path = FindPath(currentPos, targetPos);
            for (var i = 1; i < path.Count; i++)
            {
                var direction = path[i] - path[i - 1];
                var directionCode = myDirectionCodes[direction];
                Step(directionCode);
            }
            return targetPos;
        }

        private List<Point> FindPath(Point start, Point end)
        {
            var visited = new HashSet<Point>();
            var queue = new Queue<(Point, List<Point>)>(new[] { (start, new List<Point>()) });
            while (queue.Count > 0)
            {
                var (pos, path) = queue.Dequeue();
                if (!visited.Add(pos)) { continue; }

                var nextPath = path.ToList();
                nextPath.Add(pos);
                if (pos == end) { return nextPath; }

                foreach (var direction in myDirections.Values)
                {
                    var nextPos = pos + direction;
                    if (!Map.TryGetValue(nextPos, out var tile) || tile == Tile.Wall)
                    {
                        continue;
                    }
                    queue.Enqueue((nextPos, nextPath));
                }
            }

            return null;
        }

        private long Step(int directionCode)
        {
            myIntMachine.InputQueue.Enqueue(directionCode);
            myIntMachine.RunUntilBlockOrComplete();
            var tileCode = myIntMachine.OutputQueue.Dequeue();

            return tileCode;
        }

        private SynchronousIntMachine myIntMachine;

        private readonly Dictionary<int, Point> myDirections = new Dictionary<int, Point>
        {
            [1] = new Point(0, -1), // North
            [2] = new Point(0, 1),  // South
            [3] = new Point(-1, 0), // West
            [4] = new Point(1, 0)   // East
        };

        private readonly Dictionary<Point, int> myDirectionCodes;
    }
}
