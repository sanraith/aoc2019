﻿using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Monitoring Station")]
    public sealed class Day10 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var asteroids = GetAsteroids(input);
            var detectedAsteroidCounts = await GetDetectedAsteroids(asteroids);

            return detectedAsteroidCounts.Values.OrderByDescending(x => x).First().ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var asteroids = GetAsteroids(input);
            var detectedAsteroidCounts = await GetDetectedAsteroids(asteroids);
            var monitoringStation = detectedAsteroidCounts.OrderByDescending(x => x.Value).First().Key;
            var asteroidsToDestroy = asteroids.Except(new[] { monitoringStation })
                .Select(x => (Pos: x, Distance: (monitoringStation - x).Length, Angle: (x - monitoringStation).Angle.Value))
                .OrderBy(x => x.Angle)
                .ThenBy(x => x.Distance)
                .ToList();

            var destroyedCount = 0;
            while (asteroidsToDestroy.Any())
            {
                var destroyedAsteroids = new List<(Point, double, double)>();
                foreach (var (asteroidItem, index) in asteroidsToDestroy.Select((x, i) => (x, i)))
                {
                    var (pos, distance, angle) = asteroidItem;
                    if (index > 0 && Math.Abs(angle - asteroidsToDestroy[index - 1].Angle) <= double.Epsilon)
                    {
                        continue;
                    }

                    destroyedAsteroids.Add(asteroidItem);
                    destroyedCount++;
                    if (destroyedCount == 200)
                    {
                        return (pos.X * 100 + pos.Y).ToString();
                    }
                }
                asteroidsToDestroy = asteroidsToDestroy.Except(destroyedAsteroids).ToList();
            }

            throw new InvalidOperationException($"Not enough asteroids! ({destroyedCount})");
        }

        private async Task<Dictionary<Point, int>> GetDetectedAsteroids(List<Point> asteroids)
        {
            var detectedAsteroidCounts = new Dictionary<Point, int>();
            foreach (var (proposedAsteroid, index) in asteroids.Select((x, i) => (x, i)))
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(index, asteroids.Count); }

                // This is dangerous, but good enough...
                var detectedAngles = new HashSet<double>();
                foreach (var asteroid in asteroids)
                {
                    if (asteroid == proposedAsteroid) { continue; }
                    var angle = (asteroid - proposedAsteroid).Angle.Value;
                    detectedAngles.Add(angle);
                }
                detectedAsteroidCounts.Add(proposedAsteroid, detectedAngles.Count);
            }

            return detectedAsteroidCounts;
        }

        private static List<Point> GetAsteroids(string input)
        {
            var lines = GetLines(input);
            var asteroids = new List<Point>();
            foreach (var (line, y) in lines.Select((x, i) => (x, i)))
            {
                foreach (var (c, x) in line.Select((x, i) => (x, i)))
                {
                    if (c == '#') { asteroids.Add(new Point(x, y)); }
                }
            }

            return asteroids;
        }

        [DebuggerDisplay("X={X} Y={Y}")]
        internal struct Point : IEquatable<Point>
        {
            public int X { get; }

            public int Y { get; }

            public double? Angle { get; }

            public double Length => Math.Sqrt(X * X + Y * Y);

            public Point(int x, int y)
            {
                X = x;
                Y = y;
                if (X == 0 && Y == 0)
                {
                    Angle = null;
                }
                else
                {
                    Angle = Math.Atan2(-Y, X);
                    Angle = (Math.PI / 2 - Angle + 2 * Math.PI) % (2 * Math.PI);
                }
            }

            public override bool Equals(object obj) => obj is Point other ? Equals(other) : base.Equals(obj);

            public override int GetHashCode() => X ^ Y;

            public bool Equals(Point other) => X == other.X && Y == other.Y;

            public static bool operator ==(Point a, Point b) => a.Equals(b);

            public static bool operator !=(Point a, Point b) => !a.Equals(b);

            public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

            public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

            public static Point operator *(Point a, int b) => new Point(a.X * b, a.Y * b);

            public static Point Empty { get; } = new Point(0, 0);
        }
    }
}
