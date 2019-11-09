using aoc2019.Puzzles;
using aoc2019.WebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc2019.WebApp.Services
{
    public interface IPuzzleSolutionHandler
    {
        IReadOnlyDictionary<int, SolutionMetadata> Solutions { get; }

        ISolution GetSolution(int day);
    }

    public class PuzzleSolutionHandler : IPuzzleSolutionHandler
    {
        public IReadOnlyDictionary<int, SolutionMetadata> Solutions { get; }

        public PuzzleSolutionHandler()
        {
            Solutions = GatherPuzzleSolutions();
        }

        public ISolution GetSolution(int day)
        {
            if (Solutions.TryGetValue(day, out var solutionMetadata))
            {
                return (ISolution)Activator.CreateInstance(solutionMetadata.Type);
            }
            return null;
        }

        private Dictionary<int, SolutionMetadata> GatherPuzzleSolutions()
        {
            var solutionsByDay = new Dictionary<int, SolutionMetadata>();
            var solutionInterface = typeof(ISolution);
            var solutionTypes = solutionInterface.Assembly.GetTypes()
                .Where(x => solutionInterface.IsAssignableFrom(x) && !x.IsAbstract)
                .ToList();

            var numerRegex = new Regex(@"[0-9]+");
            foreach (var solutionType in solutionTypes)
            {
                var day = Convert.ToInt32(numerRegex.Match(solutionType.Name).Value);
                var name = $"{solutionType.Name} name";
                var description = "No description have been provided for this puzzle yet.";
                solutionsByDay.Add(day, new SolutionMetadata(solutionType, day, name, description));
            }

            return solutionsByDay;
        }
    }
}
