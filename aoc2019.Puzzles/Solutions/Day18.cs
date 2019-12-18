using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Many-Worlds Interpretation")]
    public sealed class Day18 : SolutionBase
    {
        public override string Part1(string input)
        {
            var map = GetMap(input);
            var entrance = Array.IndexOf(map, Entrance);
            map[entrance] = Empty;

            // 1. get possible key destinations
            // 2. for each key
            //      Go to closest key
            //      Create state from pos + keys
            //      Check if
            //          no state with same keys => add state, continue with 1.
            //          worse state with same keys => replace state, continue with 1.
            //          better or equal state with same keys => continue with 2.

            var keys = new HashSet<char>();
            var states = new Dictionary<string, int>();
            var length = DiscoverPath(map, keys, states, entrance);

            return length.ToString();
        }

        public override string Part2(string input)
        {
            throw new NotImplementedException();
        }

        private int DiscoverPath(char[] map, HashSet<char> keys, Dictionary<string, int> states, int startPos, int currentDistance = 0)
        {
            int bestDistance = int.MaxValue;
            if (currentDistance >= myBestDistance) { return bestDistance; }

            var reachableKeys = DiscoverReachableKeys(map, keys, startPos);
            foreach (var (key, (keyPos, keyDistance)) in reachableKeys.OrderBy(kd => kd.Value.Distance))
            {
                var state = GetState(key, keys);
                var totalDistance = currentDistance + keyDistance;
                if (!states.TryGetValue(state, out var storedDistance) || totalDistance < storedDistance)
                {
                    states[state] = totalDistance;
                }
                else
                {
                    continue;
                }

                var newKeys = new HashSet<char>(keys);
                newKeys.Add(key);

                if (newKeys.Count < myKeyCount)
                {
                    totalDistance = DiscoverPath(map, newKeys, states, keyPos, totalDistance);
                }
                else
                {
                    if (totalDistance < myBestDistance)
                    {
                        myBestDistance = totalDistance;
                    }
                }
                bestDistance = Math.Min(bestDistance, totalDistance);
            }

            return bestDistance;
        }

        private static string GetState(char key, HashSet<char> keys)
        {
            var sb = new StringBuilder(keys.Count + 1);
            sb.Append(keys.OrderBy(x => x).ToArray());
            sb.Append(key);
            return sb.ToString();
        }

        private Dictionary<char, (int Pos, int Distance)> DiscoverReachableKeys(char[] map, HashSet<char> keys, int startPos)
        {
            var visited = new HashSet<int>();
            var foundKeys = new Dictionary<char, (int Pos, int Distance)>();
            var queue = new Queue<(int Pos, int Distance)>(new[] { (startPos, 0) });
            while (queue.Count > 0)
            {
                var (pos, distance) = queue.Dequeue();
                var tile = map[pos];
                if (tile == Wall) { continue; }

                if (IsKey(tile) && !keys.Contains(tile) && !foundKeys.ContainsKey(tile))
                {
                    foundKeys.Add(tile, (pos, distance));
                }

                if (IsDoor(tile) && !keys.Contains(ToKey(tile)))
                {
                    continue;
                }

                foreach (var direction in myDirections)
                {
                    var nextPos = pos + direction;
                    if (!IsWithinBounds(nextPos) || !visited.Add(nextPos)) { continue; }
                    queue.Enqueue((nextPos, distance + 1));
                }
            }

            return foundKeys;
        }

        private static bool IsKey(char c) => c >= 97 && c <= 122;

        private static bool IsDoor(char c) => c >= 65 && c <= 90;

        private static char ToKey(char c) => (char)(c + 32);

        private char[] GetMap(string input)
        {
            var lines = GetLines(input);
            myWidth = lines.First().Length;
            myHeight = lines.Count;
            myMapSize = myWidth * myHeight;
            myDirections = new[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) }.Select(GetCoord).ToArray();

            var map = lines.SelectMany(x => x).ToArray();
            myKeyCount = map.Count(IsKey);
            myBestDistance = int.MaxValue;

            return map;
        }

        private int GetCoord(Point point) => point.X + point.Y * myWidth;

        private Point GetCoord(int coord) => new Point(coord % myWidth, coord / myWidth);

        private bool IsWithinBounds(int coord) => coord >= 0 && coord < myMapSize;

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
