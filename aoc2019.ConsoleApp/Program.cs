using aoc2019.Puzzles;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using CommandLine;
using CommandLine.Text;
using HtmlAgilityPack;

namespace aoc2019.ConsoleApp
{
    public sealed class Program
    {
        private sealed class Options
        {
            [Option('d', "day", HelpText = "[Number of day] Run the solution for the given day.")]
            public int? DayToRun { get; set; }

            [Option('s', "setup", HelpText = "[Number of day] Download input and description for given day, and add it to aoc2019.Puzzles.")]
            public int? DayToSetup { get; set; }

            [Usage(ApplicationAlias = "aoc2019.ConsoleApp")]
            public static IEnumerable<Example> Examples => new Example[]
            {
                new Example("Run solution for day 12", new UnParserSettings{ PreferShortName = true }, new Options { DayToRun = 12 }),
                new Example("Add input and description for day 23 to aoc2019.Puzzles", new Options { DayToSetup = 23 }),
            };
        }

        public static async Task Main(string[] args) => await new Program(args).Run();

        public Program(string[] args)
        {
            Options options = null;
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => options = o);
            myConfig = Configuration.Load();
            myOptions = options;
        }

        public async Task Run()
        {
            if (myOptions == null) { return; }

            if (myOptions.DayToRun.HasValue)
            {
                await SolveDay(myOptions.DayToRun.Value);
            }
            if (myOptions.DayToSetup.HasValue)
            {
                await SetupDay(myOptions.DayToSetup.Value);
            }
        }

        private async Task SolveDay(int day)
        {
            var solutionHandler = new SolutionHandler();
            var solution = solutionHandler.Solutions[day].CreateInstance();

            var dayString = day.ToString().PadLeft(2, '0');
            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var input = File.ReadAllText(Path.Combine(rootDir, "Input", $"day{dayString}.txt"));

            Console.WriteLine($"Solving {day}...");
            Console.WriteLine($"Part 1: {await solution.Part1(input)}");
            Console.WriteLine($"Part 2: {await solution.Part2(input)}");
        }

        private async Task SetupDay(int day)
        {
            var dayString = day.ToString().PadLeft(2, '0');
            Console.WriteLine($"Setting up input and description for {myConfig.Year}/12/{dayString}...");
            var inputAddress = $"https://adventofcode.com/{myConfig.Year}/day/{day}/input";
            var descriptionAddress = $"https://adventofcode.com/{myConfig.Year}/day/{day}";
            var puzzlesPath = myConfig.PuzzleProjectPath ?? Path.Combine("..", "aoc2019.Puzzles");
            var inputFile = new FileInfo(Path.Combine(puzzlesPath, "Input", $"day{dayString}.txt"));
            var descriptionFile = new FileInfo(Path.Combine(puzzlesPath, "Description", $"day{dayString}.html"));

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("session", myConfig.SessionCookie, "/", "adventofcode.com"));
            using var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var httpClient = new HttpClient(httpClientHandler);

            Console.WriteLine($"Downloading input from {inputAddress}");
            var input = await httpClient.GetStringAsync(inputAddress);

            Console.WriteLine($"Saving input to {inputFile.FullName}");
            File.WriteAllText(inputFile.FullName, input, Encoding.UTF8);

            Console.WriteLine($"Downloading description from {descriptionAddress}");
            var descriptionPageSource = await httpClient.GetStringAsync(descriptionAddress);
            var descriptionPage = new HtmlDocument();
            descriptionPage.LoadHtml(descriptionPageSource);
            var articleNodes = descriptionPage.DocumentNode.SelectNodes("//article[@class='day-desc']");
            Console.WriteLine($"Found {articleNodes.Count} parts.");
            var description = articleNodes.Aggregate(string.Empty, (result, node) => result + node.OuterHtml);

            Console.WriteLine($"Saving description to {descriptionFile.FullName}");
            File.WriteAllText(descriptionFile.FullName, description, Encoding.UTF8);

            Console.WriteLine("Done.");
        }

        private readonly Options myOptions;
        private readonly Configuration myConfig;
    }
}
