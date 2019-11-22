using aoc2019.Puzzles;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace aoc2019.ConsoleApp
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            var day = args.Any() ? Convert.ToInt32(args.First()) : 1;

            var solutionHandler = new SolutionHandler();
            var solution = solutionHandler.Solutions[day].CreateInstance();

            var dayString = day.ToString().PadLeft(2, '0');
            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var input = File.ReadAllText(Path.Combine(rootDir, "Input", $"day{dayString}.txt"));

            Console.WriteLine($"Solving {day}...");
            Console.WriteLine($"Part 1: {await solution.Part1(input)}");
            Console.WriteLine($"Part 2: {await solution.Part2(input)}");
        }
    }
}
