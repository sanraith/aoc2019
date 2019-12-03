using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Crossed Wires")]
    public sealed class Day03 : SolutionBase
    {
        public override string Part1(string input)
        {
            var wires = ParseWires(input);

            throw new NotImplementedException();
        }

        public override string Part2(string input)
        {
            throw new NotImplementedException();
        }

        private Point? GetIntersection(ReadOnlySpan<Point> sectionA, ReadOnlySpan<Point> sectionB)
        {

            return null;
        }

        private IReadOnlyList<Point[]> ParseWires(string input)
        {
            var regex = new Regex(@"(?'direction'[A-Z])(?'distance'[0-9]+)");
            var lines = GetLines(input);
            var wires = new List<List<Point>>();
            foreach (var line in lines)
            {
                var pos = new Point(0, 0);
                var wire = new List<Point> { pos };
                foreach (var match in regex.Matches(line).OfType<Match>())
                {
                    var direction = match.Groups["direction"].Value.First();
                    var distance = Convert.ToInt32(match.Groups["distance"].Value);
                    switch (direction)
                    {
                        case 'U': pos = new Point(pos.X, pos.Y + distance); break;
                        case 'R': pos = new Point(pos.X + distance, pos.Y); break;
                        case 'D': pos = new Point(pos.X, pos.Y - distance); break;
                        case 'L': pos = new Point(pos.X - distance, pos.Y); break;
                    }
                    wire.Add(pos);
                }
                wires.Add(wire);
            }

            return wires.Select(x => x.ToArray()).ToList();
        }
    }
}
