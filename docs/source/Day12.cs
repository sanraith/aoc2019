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
            var originalMoons = GetMoons(input);
            var moons = GetMoons(input);

            var stepCount = 0;
            var intervals = new int[3];
            var completed = false;
            while (!completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(intervals.Count(x => x > 0), 3); }

                Step(moons);
                stepCount++;

                for (var axis = 0; axis < 3; axis++)
                {
                    if (intervals[axis] == 0 && AreAxisStatesEqual(originalMoons, moons, (Axis)axis))
                    {
                        intervals[axis] = stepCount;
                        if (intervals.All(x => x > 0))
                        {
                            completed = true;
                            break;
                        }
                    }
                }
            }

            var lcm = GetLeastCommonMultiple(intervals);
            return lcm.ToString();
        }

        public static long GetLeastCommonMultiple(int[] elements)
        {
            long lcm = 1;
            var divisor = 2;
            while (true)
            {
                var counter = 0;
                var divisible = false;
                for (var i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == 0) { return 0; }
                    else if (elements[i] < 0) { elements[i] = elements[i] * (-1); }

                    if (elements[i] == 1) { counter++; }
                    if (elements[i] % divisor == 0)
                    {
                        divisible = true;
                        elements[i] = elements[i] / divisor;
                    }
                }

                if (divisible) { lcm = lcm * divisor; }
                else { divisor++; }

                if (counter == elements.Length) { return lcm; }
            }
        }

        private static bool AreAxisStatesEqual(Moon[] original, Moon[] current, Axis axis)
        {
            var length = original.Length;
            for (var i = 0; i < length; i++)
            {
                bool falseCondition;
                switch (axis)
                {
                    case Axis.X: falseCondition = original[i].Pos.X != current[i].Pos.X || original[i].Velocity.X != current[i].Velocity.X; break;
                    case Axis.Y: falseCondition = original[i].Pos.Y != current[i].Pos.Y || original[i].Velocity.Y != current[i].Velocity.Y; break;
                    case Axis.Z: falseCondition = original[i].Pos.Z != current[i].Pos.Z || original[i].Velocity.Z != current[i].Velocity.Z; break;
                    default: throw new InvalidOperationException();
                }
                if (falseCondition) { return false; }
            }
            return true;
        }

        private static void Step(Moon[] moons)
        {
            var length = moons.Length;
            for (var indexA = 0; indexA < length; indexA++)
            {
                var moonA = moons[indexA];
                for (var indexB = indexA + 1; indexB < length; indexB++)
                {
                    var moonB = moons[indexB];
                    var diffX = moonA.Pos.X.CompareTo(moonB.Pos.X);
                    var diffY = moonA.Pos.Y.CompareTo(moonB.Pos.Y);
                    var diffZ = moonA.Pos.Z.CompareTo(moonB.Pos.Z);

                    var gravity = new Vector3D(diffX, diffY, diffZ);
                    moonA.Velocity += gravity * -1;
                    moonB.Velocity += gravity;
                }
                moonA.Pos += moonA.Velocity;
            }
        }

        private static Moon[] GetMoons(string input)
        {
            var regex = new Regex(@"x=(?'x'-?[0-9]+), y=(?'y'-?[0-9]+), z=(?'z'-?[0-9]+)");
            var points = new List<Point3D>();
            foreach (var match in regex.Matches(input).OfType<Match>())
            {
                var (x, y, z) = match.Groups.OfType<Group>().Skip(1).Select(g => Convert.ToInt32(g.Value));
                points.Add(new Point3D(x, y, z));
            }

            return points.Select(p => new Moon(p)).ToArray();
        }

        private enum Axis { X = 0, Y = 1, Z = 2 }

        private sealed class Moon
        {
            public Point3D Pos { get; set; }

            public Vector3D Velocity { get; set; } = new Vector3D(0, 0, 0);

            public Moon(Point3D pos) => Pos = pos;
        }
    }
}
