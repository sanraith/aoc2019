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
using System.Text.RegularExpressions;

namespace aoc2019.ConsoleApp
{
    public sealed class Program
    {
        private sealed class Options
        {
            [Option('d', "day", HelpText = "[Number of day] Run the solution for the given day.")]
            public int? DayToRun { get; set; }

            [Option('s', "setup", HelpText = "[Number of day] Download input and description for given day, and add it to aoc2019.Puzzles along with an empty solution .cs file.")]
            public int? DayToSetup { get; set; }

            [Usage(ApplicationAlias = "aoc2019.ConsoleApp")]
            public static IEnumerable<Example> Examples => new Example[]
            {
                new Example("Run solution for day 12", new UnParserSettings{ PreferShortName = true }, new Options { DayToRun = 12 }),
                new Example("Add input and description for day 23 to aoc2019.Puzzles along with an empty solution .cs file", new Options { DayToSetup = 23 }),
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
            var puzzleProjectPath = myConfig.PuzzleProjectPath ?? Path.Combine("..", "aoc2019.Puzzles");
            Console.WriteLine($"Setting up input and description for {myConfig.Year}/12/{dayString}...");

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("session", myConfig.SessionCookie, "/", "adventofcode.com"));
            using var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var httpClient = new HttpClient(httpClientHandler);

            await SaveInputAsync(day, dayString, puzzleProjectPath, httpClient);
            string puzzleTitle = await SaveDescriptionAsync(day, dayString, puzzleProjectPath, httpClient);
            await CreateSolutionSourceAsync(day, dayString, puzzleProjectPath, puzzleTitle);

            Console.WriteLine("Done.");
        }

        private async Task SaveInputAsync(int day, string dayString, string puzzleProjectPath, HttpClient httpClient)
        {
            var inputAddress = $"https://adventofcode.com/{myConfig.Year}/day/{day}/input";
            var inputFile = new FileInfo(Path.Combine(puzzleProjectPath, "Input", $"day{dayString}.txt"));
            Console.WriteLine($"Downloading input from {inputAddress}");
            var input = await httpClient.GetStringAsync(inputAddress);

            Console.WriteLine($"Saving input to {inputFile.FullName}");
            File.WriteAllText(inputFile.FullName, input, Encoding.UTF8);
        }

        private async Task<string> SaveDescriptionAsync(int day, string dayString, string puzzleProjectPath, HttpClient httpClient)
        {
            var descriptionAddress = $"https://adventofcode.com/{myConfig.Year}/day/{day}";
            var descriptionFile = new FileInfo(Path.Combine(puzzleProjectPath, "Descriptions", $"day{dayString}.html"));
            var puzzleTitleRegex = new Regex(@"---.*: (?'title'.*) ---");

            Console.WriteLine($"Downloading description from {descriptionAddress}");
            var descriptionPageSource = await httpClient.GetStringAsync(descriptionAddress);
            var descriptionPage = new HtmlDocument();
            descriptionPage.LoadHtml(descriptionPageSource);
            var articleNodes = descriptionPage.DocumentNode.SelectNodes("//article[@class='day-desc']");

            var titleNode = articleNodes.First().SelectSingleNode("//h2");
            var puzzleTitle = puzzleTitleRegex.Match(titleNode.InnerText).Groups["title"].Value;
            titleNode.InnerHtml = $"--- Part One ---";
            Console.WriteLine($"Found {articleNodes.Count} parts. Title: {puzzleTitle}");
            var description = articleNodes.Aggregate(string.Empty, (result, node) => result + node.OuterHtml);

            Console.WriteLine($"Saving description to {descriptionFile.FullName}");
            File.WriteAllText(descriptionFile.FullName, description, Encoding.UTF8);

            return puzzleTitle;
        }

        private static async Task CreateSolutionSourceAsync(int day, string dayString, string puzzleProjectPath, string puzzleTitle)
        {
            var consoleProjectBinPath = GetApplicationRoot();
            var solutionSourceFile = new FileInfo(Path.Combine(consoleProjectBinPath, "Template", $"Day_DAYSTRING_.cs"));
            var solutionTargetFile = new FileInfo(Path.Combine(puzzleProjectPath, "Solutions", $"Day{dayString}.cs"));
            if (solutionTargetFile.Exists)
            {
                Console.WriteLine($"Source file already exists at {solutionTargetFile.FullName}");
            }
            else
            {
                Console.WriteLine($"Saving source file to {solutionTargetFile.FullName}");
                var content = await File.ReadAllTextAsync(solutionSourceFile.FullName);
                content = content
                    .Replace("_DAYNUMBER_", day.ToString())
                    .Replace("_DAYSTRING_", dayString)
                    .Replace("_PUZZLETITLE_", puzzleTitle);
                await File.WriteAllTextAsync(solutionTargetFile.FullName, content, Encoding.UTF8);
            }
        }

        private static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        private readonly Options myOptions;
        private readonly Configuration myConfig;
    }
}
