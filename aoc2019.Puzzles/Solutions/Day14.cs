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
        public override async Task<string> Part1Async(string input)
        {
            ParseInput(input);
            throw new NotImplementedException();
        }

        public override async Task<string> Part2Async(string input)
        {
            throw new NotImplementedException();
        }

        private void ParseInput(string input)
        {
            var lineRegex = new Regex(@"(?'sources'(?:(?:[0-9]+) (?:[A-Z]+)(?:, )?)+) => (?'count'[0-9]+) (?'result'[A-Z]+)");
            var sourcesRegex = new Regex(@"(?'count'[0-9]+) (?'name'[A-Z]+)");
            foreach (Match lineMatch in lineRegex.Matches(input))
            {
                var result = lineMatch.Groups["result"].Value;
                var resultCount = Convert.ToInt32(lineMatch.Groups["count"].Value);
                var sources = sourcesRegex.Matches(lineMatch.Groups["sources"].Value).OfType<Match>()
                    .Select(x => (Name: x.Groups["name"].Value, Count: Convert.ToInt32(x.Groups["count"].Value)))
                    .ToList();
            }
        }
    }
}
