using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Space Stoichiometry")]
    public sealed class Day14 : SolutionBase
    {
        public override string Part1(string input)
        {
            var chemicals = ParseChemicals(input);
            var requiredOreCount = CraftFuel(chemicals, 1);

            return requiredOreCount.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            const long targetOreCount = 1000000000000;
            var chemicals = ParseChemicals(input);
            var originalOreCount = CraftFuel(chemicals, 1);

            long fuelCount = targetOreCount / originalOreCount;
            long min = 0;
            while (CraftFuel(chemicals, fuelCount) < targetOreCount) { min = fuelCount; fuelCount *= 2; }
            var max = fuelCount;
            var searchSpace = max - min;

            long targetFuelCount = 0;
            while (min < max)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(searchSpace - max + min, searchSpace); }

                fuelCount = (max - min) / 2 + min;
                var oreCount = CraftFuel(chemicals, fuelCount);
                if (oreCount < targetOreCount)
                {
                    targetFuelCount = fuelCount;
                    min = fuelCount + 1;
                }
                else
                {
                    max = fuelCount;
                }
            }

            return targetFuelCount.ToString();
        }

        private static long CraftFuel(Dictionary<string, Chemical> chemicals, long fuelCount)
        {
            var stock = chemicals.Values.ToDictionary(k => k, v => (long)0);
            var fuel = chemicals[Fuel];
            var ore = chemicals[Ore];
            long requiredOreCount = 0;

            var craftingStack = new Stack<(Chemical chemical, long Count)>();
            fuel.Recipe.Ingredients.ForEach(i => craftingStack.Push((i.Chemical, i.Count * fuelCount)));

            while (craftingStack.Count > 0)
            {
                var (chemical, requiredAmount) = craftingStack.Pop();
                if (chemical == ore)
                {
                    requiredOreCount += requiredAmount;
                    continue;
                }

                var currentStock = stock[chemical];
                if (currentStock >= requiredAmount)
                {
                    stock[chemical] -= requiredAmount;
                }
                else
                {
                    var batchSize = chemical.Recipe.ResultCount;
                    var recipeCount = (long)Math.Ceiling((requiredAmount - currentStock) / (double)batchSize);
                    stock[chemical] += recipeCount * batchSize - requiredAmount;
                    foreach (var ingredient in chemical.Recipe.Ingredients)
                    {
                        craftingStack.Push((ingredient.Chemical, recipeCount * ingredient.Count));
                    }
                }
            }

            return requiredOreCount;
        }

        private Dictionary<string, Chemical> ParseChemicals(string input)
        {
            var lineRegex = new Regex(@"(?'sources'(?:(?:[0-9]+) (?:[A-Z]+)(?:, )?)+) => (?'count'[0-9]+) (?'result'[A-Z]+)");
            var sourcesRegex = new Regex(@"(?'count'[0-9]+) (?'name'[A-Z]+)");

            var chemicals = new Dictionary<string, Chemical>();
            foreach (Match lineMatch in lineRegex.Matches(input))
            {
                var result = lineMatch.Groups["result"].Value;
                var resultCount = Convert.ToInt32(lineMatch.Groups["count"].Value);
                var sources = sourcesRegex.Matches(lineMatch.Groups["sources"].Value).OfType<Match>()
                    .Select(x => (Name: x.Groups["name"].Value, Count: Convert.ToInt32(x.Groups["count"].Value)))
                    .ToList();

                var chemical = chemicals.GetOrAdd(result, n => new Chemical(n));
                chemical.Recipe = new Recipe(chemical, resultCount);
                foreach (var (sourceName, sourceCount) in sources)
                {
                    var sourceChemical = chemicals.GetOrAdd(sourceName, n => new Chemical(n));
                    chemical.Recipe.Ingredients.Add((sourceChemical, sourceCount));
                }
            }

            return chemicals;
        }

        private sealed class Chemical
        {
            public string Name { get; }

            public Recipe Recipe { get; set; }

            public Chemical(string name) => Name = name;

            public override string ToString() => Name;
        }

        private sealed class Recipe
        {
            public Chemical Result { get; }

            public int ResultCount { get; set; }

            public List<(Chemical Chemical, int Count)> Ingredients { get; } = new List<(Chemical Chemical, int Count)>();

            public Recipe(Chemical result, int resultCount)
            {
                Result = result;
                ResultCount = resultCount;
            }

            public override string ToString() => $"{string.Join(", ", Ingredients.Select(i => i.Count + " " + i.Chemical.Name))} => {ResultCount} {Result.Name}";
        }

        private const string Ore = "ORE";
        private const string Fuel = "FUEL";
    }
}
