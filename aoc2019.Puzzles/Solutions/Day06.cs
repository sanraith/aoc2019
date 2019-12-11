using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Universal Orbit Map")]
    public sealed class Day06 : SolutionBase
    {
        public override string Part1(string input)
        {
            var planets = ParsePlanets(input);
            var sum = planets.Values.Sum(p => p.DirectOrbitCount + p.IndirectOrbitCount);

            return sum.ToString();
        }

        public override string Part2(string input)
        {
            var planets = ParsePlanets(input);
            var pathFromYou = GetPathToCenterOfMass(planets[You]);
            var pathFromSanta = GetPathToCenterOfMass(planets[Santa]);
            var commonParent = pathFromYou.Intersect(pathFromSanta).First();
            var transferCount = pathFromYou.IndexOf(commonParent) + pathFromSanta.IndexOf(commonParent);

            return transferCount.ToString();
        }

        private List<Planet> GetPathToCenterOfMass(Planet planet)
        {
            var path = new List<Planet>();
            while (planet.Orbits != null)
            {
                planet = planet.Orbits;
                path.Add(planet);
            }

            return path;
        }

        private IDictionary<string, Planet> ParsePlanets(string input)
        {
            var planets = new Dictionary<string, Planet>();
            foreach (var line in GetLines(input))
            {
                var pair = line.Trim().Split(')');

                var parentPlanet = planets.GetOrAdd(pair[0], name => new Planet(name));
                var childPlanet = planets.GetOrAdd(pair[1], name => new Planet(name));
                childPlanet.Orbits = parentPlanet;
            }

            return planets;
        }

        [DebuggerDisplay("{Name}")]
        private sealed class Planet
        {
            public string Name { get; }

            public Planet Orbits { get; set; }

            public int DirectOrbitCount => Orbits == null ? 0 : 1;

            public int IndirectOrbitCount
            {
                get
                {
                    if (myIndirectOrbitCount == null)
                    {
                        myIndirectOrbitCount = Orbits == null ? 0 : Orbits.DirectOrbitCount + Orbits.IndirectOrbitCount;
                    }
                    return myIndirectOrbitCount.Value;
                }
            }

            public Planet(string name) => Name = name;

            private int? myIndirectOrbitCount;
        }

        private const string You = "YOU";
        private const string Santa = "SAN";
    }
}
