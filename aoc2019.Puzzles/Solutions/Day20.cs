using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day10;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Donut Maze")]
    public sealed class Day20 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            throw new NotImplementedException();
        }

        public override async Task<string> Part2Async(string input)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Point, Point> GetPortals(Dictionary<Point, char> map)
        {
            foreach (var (pos, c) in map)
            {
                if (c < 'A' || c > 'Z') { continue; }

                var connection = Directions.Select(d => (Point?)(pos + d)).FirstOrDefault(p => map[p.Value] == '.');
                if (connection == null) { continue; }

                var otherPos = Directions.Select(d => pos + d).FirstOrDefault(p => map[p] >= 'A' && map[p] <= 'Z');
            }
            return null;
        }

        private Dictionary<Point, char> ParseMap(string input)
        {
            var lines = GetLines(input);
            var map = new Dictionary<Point, char>();
            foreach (var (line, y) in lines.WithIndex())
            {
                foreach (var (c, x) in line.WithIndex())
                {
                    if (c == ' ') { continue; }
                    map[new Point(x, y)] = c;
                }
            }

            return map;
        }

        private static readonly Point[] Directions = new[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
    }
}
