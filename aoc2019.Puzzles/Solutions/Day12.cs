using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("The N-Body Problem")]
    public sealed class Day12 : SolutionBase
    {
        public int Part1StepCount { get; set; } = 1000;

        public override string Part1(string input)
        {
            var moons = GetMoons(input);

            for (var iteration = 0; iteration < Part1StepCount; iteration++)
            {
                Step(moons);
            }

            var totalEnergy = 0;
            foreach (var moon in moons)
            {
                var energy = new[] { moon.Pos.X, moon.Pos.Y, moon.Pos.Z }.Select(Math.Abs).Sum();
                energy *= new[] { moon.Velocity.X, moon.Velocity.Y, moon.Velocity.Z }.Select(Math.Abs).Sum();
                totalEnergy += (int)energy;
            }

            return totalEnergy.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var moons = GetMoons(input);
            var xState = moons.Select(x => x.Pos.X).Concat(moons.Select(x => x.Velocity.X)).ToArray();
            var yState = moons.Select(x => x.Pos.Y).Concat(moons.Select(x => x.Velocity.Y)).ToArray();
            var zState = moons.Select(x => x.Pos.Z).Concat(moons.Select(x => x.Velocity.Z)).ToArray();

            var cycles = new int[3];

            var step = 0;
            while (true)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(cycles.Count(x => x > 0), 3); }
                Step(moons);
                step++;

                if (cycles[0] == 0 && xState.SequenceEqual(moons.Select(m => m.Pos.X).Concat(moons.Select(m => m.Velocity.X))))
                {
                    cycles[0] = step;
                    if (cycles.All(x => x > 0)) { break; }
                }
                if (cycles[1] == 0 && yState.SequenceEqual(moons.Select(m => m.Pos.Y).Concat(moons.Select(m => m.Velocity.Y))))
                {
                    cycles[1] = step;
                    if (cycles.All(x => x > 0)) { break; }
                }
                if (cycles[2] == 0 && zState.SequenceEqual(moons.Select(m => m.Pos.Z).Concat(moons.Select(m => m.Velocity.Z))))
                {
                    cycles[2] = step;
                    if (cycles.All(x => x > 0)) { break; }
                }
            }
            var lcm = GetLeastCommonMultiple(cycles);

            return lcm.ToString();
        }

        public static long GetLeastCommonMultiple(int[] elements)
        {
            long lcm = 1;
            int divisor = 2;

            while (true)
            {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == 0)
                    {
                        continue; // Do not consider zero.
                    }
                    else if (elements[i] < 0)
                    {
                        elements[i] = elements[i] * (-1);
                    }
                    if (elements[i] == 1)
                    {
                        counter++;
                    }
                    if (elements[i] % divisor == 0)
                    {
                        divisible = true;
                        elements[i] = elements[i] / divisor;
                    }
                }
                if (divisible)
                {
                    lcm = lcm * divisor;
                }
                else
                {
                    divisor++;
                }
                if (counter == elements.Length)
                {
                    return lcm;
                }
            }
        }

        private static void Step(List<Moon> moons)
        {
            foreach (var (moonA, index) in moons.WithIndex())
            {
                foreach (var moonB in moons.Skip(index + 1))
                {
                    var diffX = moonA.Pos.X.CompareTo(moonB.Pos.X);
                    var diffY = moonA.Pos.Y.CompareTo(moonB.Pos.Y);
                    var diffZ = moonA.Pos.Z.CompareTo(moonB.Pos.Z);

                    var gravity = new Vector3D(diffX, diffY, diffZ);
                    moonA.Velocity += gravity * -1;
                    moonB.Velocity += gravity;
                }
            }

            foreach (var moon in moons)
            {
                moon.Pos += moon.Velocity;
            }
        }

        private static List<Moon> GetMoons(string input)
        {
            var regex = new Regex(@"x=(?'x'-?[0-9]+), y=(?'y'-?[0-9]+), z=(?'z'-?[0-9]+)");
            var points = new List<Point3D>();
            foreach (var match in regex.Matches(input).OfType<Match>())
            {
                var (x, y, z) = match.Groups.OfType<Group>().Skip(1).Select(g => Convert.ToInt32(g.Value));
                points.Add(new Point3D(x, y, z));
            }

            return points.Select(p => new Moon(p)).ToList();
        }

        private sealed class Moon
        {
            public Point3D Pos { get; set; }

            public Vector3D Velocity { get; set; } = new Vector3D(0, 0, 0);

            public Moon(Point3D pos) => Pos = pos;
        }
    }
}
