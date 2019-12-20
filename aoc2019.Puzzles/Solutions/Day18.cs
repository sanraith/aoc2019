using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Many-Worlds Interpretation")]
    public sealed class Day18 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var map = GetMap(input);
            var entrance = map.WithIndex().Where(x => x.Item == Entrance).Select(x => x.Index).Single();
            map[entrance] = Empty;
            var fewestSteps = await Solve(map, new[] { entrance });

            return fewestSteps.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var map = GetMap(input);
            var robotPositions = ConvertEntrances(map);
            var fewestSteps = await Solve(map, robotPositions);

            return fewestSteps.ToString();
        }

        private async Task<int> Solve(char[] map, int[] robotPositions)
        {
            myBestDistance = int.MaxValue;
            var keysByChar = GetDistances(map);
            var relativeKeysByPos = keysByChar.Values.ToDictionary(k => k.Pos, v => v.RelativeKeys.Values.ToArray());
            robotPositions.ToList().ForEach(p => relativeKeysByPos.Add(p, DiscoverRelativeKeys(map, p).Values.ToArray()));

            await Backtrack(robotPositions, new List<char>(), relativeKeysByPos, new Dictionary<string, int>(), 0);

            return myBestDistance;
        }

        private async Task Backtrack(int[] robotPositions, List<char> keys, Dictionary<int, RelativeKey[]> relativeKeysByPos, Dictionary<string, int> states, int currentDistance = 0)
        {
            if (currentDistance >= myBestDistance) { return; }
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(states.Count, robotPositions.Length == 1 ? 50000 : 200000); }
            if (keys.Count == myKeyCount && currentDistance < myBestDistance)
            {
                myBestDistance = currentDistance;
                return;
            }

            if (currentDistance > 0)
            {
                long posState = 0;
                foreach (var robotPosition in robotPositions)
                {
                    posState *= myMapSize;
                    posState += robotPosition;
                }
                var state = posState + string.Join("", keys.OrderBy(x => x));
                if (states.TryGetValue(state, out var storedDistance) && storedDistance <= currentDistance)
                {
                    return;
                }
                states[state] = currentDistance;
            }

            foreach (var (pos, id) in robotPositions.WithIndex())
            {
                var relativeKeys = relativeKeysByPos[pos];
                var reachableKeys = relativeKeys
                    .Where(r => !keys.Contains(r.Char) && !r.RequiredKeys.Except(keys).Any())
                    .OrderBy(x => x.Distance);
                foreach (var destination in reachableKeys)
                {
                    robotPositions[id] = destination.Pos;
                    keys.Add(destination.Char);
                    await Backtrack(robotPositions, keys, relativeKeysByPos, states, currentDistance + destination.Distance);
                    keys.RemoveAt(keys.Count - 1);
                }
                robotPositions[id] = pos;
            }
        }

        private Dictionary<char, Key> GetDistances(char[] map)
        {
            return map
                .Select((t, p) => (Tile: t, Pos: p))
                .Where(x => IsKey(x.Tile))
                .ToDictionary(k => k.Tile, v => new Key(v.Tile, v.Pos, DiscoverRelativeKeys(map, v.Pos)));
        }

        private Dictionary<char, RelativeKey> DiscoverRelativeKeys(char[] map, int startPos)
        {
            var originalKey = map[startPos];
            var visited = new HashSet<int>();
            var foundKeys = new Dictionary<char, RelativeKey>();
            var queue = new Queue<(int Pos, int Distance, char[] Doors)>(new[] { (startPos, 0, Array.Empty<char>()) });
            while (queue.Count > 0)
            {
                var (pos, distance, doors) = queue.Dequeue();
                var tile = map[pos];
                if (tile == Wall) { continue; }

                if (IsKey(tile) && !foundKeys.ContainsKey(tile) && tile != originalKey)
                {
                    foundKeys.Add(tile, new RelativeKey(tile, pos, distance, doors.Select(DoorToKey).ToArray()));
                }

                if (IsDoor(tile))
                {
                    doors = doors.Append(tile).ToArray();
                }

                foreach (var direction in myDirections)
                {
                    var nextPos = pos + direction;
                    if (!IsWithinBounds(nextPos) || !visited.Add(nextPos)) { continue; }
                    queue.Enqueue((nextPos, distance + 1, doors));
                }
            }

            return foundKeys;
        }

        private int[] ConvertEntrances(char[] map)
        {
            var entrance = Array.IndexOf(map, Entrance);
            var topLeft = GetPoint(entrance + myDirections[0] + myDirections[3]);
            var bottomRight = GetPoint(entrance + myDirections[1] + myDirections[2]);
            for (var x = topLeft.X; x <= bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y <= bottomRight.Y; y++)
                {
                    if (new[] { topLeft.X, bottomRight.X }.Contains(x) &&
                        new[] { topLeft.Y, bottomRight.Y }.Contains(y))
                    {
                        map[GetCoord(x, y)] = '@';
                    }
                    else
                    {
                        map[GetCoord(x, y)] = '#';
                    }
                }
            }

            var entrances = map.WithIndex().Where(x => x.Item == Entrance).Select(x => x.Index).ToArray();
            entrances.ToList().ForEach(p => map[p] = Empty);

            return entrances;
        }

        private static bool IsKey(char tile) => tile >= 97 && tile <= 122;

        private static bool IsDoor(char tile) => tile >= 65 && tile <= 90;

        private static char DoorToKey(char door) => (char)(door + 32);

        private char[] GetMap(string input)
        {
            var lines = GetLines(input);
            myWidth = lines.First().Length;
            myHeight = lines.Count;
            myMapSize = myWidth * myHeight;
            myDirections = new[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) }.Select(GetCoord).ToArray();

            var map = lines.SelectMany(x => x).ToArray();
            myKeyCount = map.Count(IsKey);

            return map;
        }

        private int GetCoord(Point point) => GetCoord(point.X, point.Y);

        private int GetCoord(int x, int y) => x + y * myWidth;

        private Point GetPoint(int coord) => new Point(coord % myWidth, coord / myWidth);

        private bool IsWithinBounds(int coord) => coord >= 0 && coord < myMapSize;

        private sealed class Key
        {
            public char Char { get; }
            public int Pos { get; }
            public Dictionary<char, RelativeKey> RelativeKeys { get; }

            public Key(char key, int pos, Dictionary<char, RelativeKey> relativeKeys)
            {
                Char = key;
                Pos = pos;
                RelativeKeys = relativeKeys;
            }
        }

        private sealed class RelativeKey
        {
            public char Char { get; }
            public int Pos { get; }
            public int Distance { get; }
            public char[] RequiredKeys { get; }

            public RelativeKey(char key, int pos, int distance, char[] requiredKeys)
            {
                Char = key;
                Pos = pos;
                Distance = distance;
                RequiredKeys = requiredKeys;
            }
        }

        private int myWidth;
        private int myHeight;
        private int myMapSize;
        private int myKeyCount;
        private int myBestDistance;
        private int[] myDirections;

        private const char Entrance = '@';
        private const char Empty = '.';
        private const char Wall = '#';
    }
}
