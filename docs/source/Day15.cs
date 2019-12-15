using aoc2019.Puzzles.Core;
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
            PathToOxygenGenerator = new List<Point>();
            await Backtrack(new Point(0, 0), null);
            PathToOxygenGenerator.Reverse();
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
                if (Map.ContainsKey(nextPos)) { continue; }

                long tileCode = Step(directionCode);
                switch (tileCode)
                {
                    case 0:
                        Map[nextPos] = Tile.Wall;
                        break;
                    case 1:
                        Map[nextPos] = Tile.Empty;
                        if (await Backtrack(nextPos, direction * -1))
                        {
                            PathToOxygenGenerator.Add(pos);
                            foundPath = true;
                        }
                        break;
                    case 2:
                        Map[nextPos] = Tile.OxygenSystem;
                        PathToOxygenGenerator.Add(pos);
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

        private readonly Dictionary<int, Point> myDirections = new Dictionary<int, Point>
        {
            [1] = new Point(0, -1), // North
            [2] = new Point(0, 1),  // South
            [3] = new Point(-1, 0), // West
            [4] = new Point(1, 0)   // East
        };
    }
}
