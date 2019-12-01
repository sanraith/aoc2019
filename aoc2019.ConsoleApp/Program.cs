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
using aoc2019.Puzzles.Core;

namespace aoc2019.ConsoleApp
{
    public sealed class Program
    {
        private sealed class Options
        {
            [Option('l', "last", HelpText = "Run the last available solution.")]
            public bool RunLastDay { get; set; }

            [Option('d', "day", HelpText = "[Number of day] Run the solution for the given day.")]
            public int? DayToRun { get; set; }

            [Option('s', "setup", HelpText = "[Number of day] Download input and description for given day, and add it to aoc2019.Puzzles along with an empty solution .cs file.")]
            public int? DayToSetup { get; set; }

            [Usage(ApplicationAlias = "aoc2019.ConsoleApp")]
            public static IEnumerable<Example> Examples => new Example[]
            {
                new Example("Run the last available solution", new Options { RunLastDay = true }),
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
            mySolutionHandler = new SolutionHandler();
        }

        public async Task Run()
        {
            if (myOptions == null) { return; }

            if (myOptions.RunLastDay)
            {
                await SolveLastDay();
            }
            if (myOptions.DayToRun.HasValue)
            {
                await SolveDay(myOptions.DayToRun.Value);
            }
            if (myOptions.DayToSetup.HasValue)
            {
                await SetupDay(myOptions.DayToSetup.Value);
            }
        }

        private async Task SolveLastDay()
        {
            var lastSolutionDay = mySolutionHandler.Solutions.Keys.LastOrDefault(x => x >= 1 && x <= 25);
            if (lastSolutionDay > 0)
            {
                await SolveDay(lastSolutionDay);
            }
            else
            {
                Console.WriteLine("No solution is available yet.");
            }
        }

        private async Task SolveDay(int day)
        {
            var solution = mySolutionHandler.Solutions[day].CreateInstance();

            var dayString = day.ToString().PadLeft(2, '0');
            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var input = File.ReadAllText(Path.Combine(rootDir, "Input", $"day{dayString}.txt"));

            Console.WriteLine($"Solving {day}...");
            await SolvePart(1, input, solution.Part1Async);
            await SolvePart(2, input, solution.Part2Async);
        }

        private static async Task SolvePart(int partNumber, string input, Func<string, Task<string>> action)
        {
            try
            {
                Console.Write($"Part {partNumber}: ");
                Console.WriteLine(await action(input));
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Not implemented.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task SetupDay(int day)
        {
            var dayString = day.ToString().PadLeft(2, '0');
            var consoleProjectBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var puzzleProjectPath = Path.Combine(consoleProjectBinPath, myConfig.PuzzleProjectPath);
            Console.WriteLine($"Setting up input and description for {myConfig.Year}/12/{dayString}...");

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("session", myConfig.SessionCookie, "/", "adventofcode.com"));
            using var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var httpClient = new HttpClient(httpClientHandler);

            await SaveInputAsync(day, dayString, puzzleProjectPath, httpClient);
            string puzzleTitle = await SaveDescriptionAsync(day, dayString, puzzleProjectPath, httpClient);
            await CreateSolutionSourceAsync(day, dayString, consoleProjectBinPath, puzzleProjectPath, puzzleTitle);

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

        private static async Task CreateSolutionSourceAsync(int day, string dayString, string consoleProjectBinPath, string puzzleProjectPath, string puzzleTitle)
        {
            var solutionSourceFile = new FileInfo(Path.Combine(consoleProjectBinPath, "Template", $"Day_DAYSTRING_.cs"));
            var solutionTargetFile = new FileInfo(Path.Combine(puzzleProjectPath, "Solutions", $"Day{dayString}.cs"));
            var testSourceFile = new FileInfo(Path.Combine(consoleProjectBinPath, "Template", $"Day_DAYSTRING_Test.cs"));
            var testTargetFile = new FileInfo(Path.Combine($"{puzzleProjectPath}.Test", $"Day{dayString}Test.cs"));

            if (solutionTargetFile.Exists)
            {
                Console.WriteLine($"Source file already exists at {solutionTargetFile.FullName}");
            }
            else
            {
                Console.WriteLine($"Saving source file to {solutionTargetFile.FullName}");
                var sourceContent = await File.ReadAllTextAsync(solutionSourceFile.FullName);
                sourceContent = sourceContent
                    .Replace("_DAYNUMBER_", day.ToString())
                    .Replace("_DAYSTRING_", dayString)
                    .Replace("_PUZZLETITLE_", puzzleTitle);
                await File.WriteAllTextAsync(solutionTargetFile.FullName, sourceContent, Encoding.UTF8);
            }

            if (testTargetFile.Exists)
            {
                Console.WriteLine($"Test file already exists at {solutionTargetFile.FullName}");
            }
            else
            {

                Console.WriteLine($"Saving test file to {testTargetFile.FullName}");
                var testContent = await File.ReadAllTextAsync(testSourceFile.FullName);
                testContent = testContent.Replace("_DAYSTRING_", dayString);
                await File.WriteAllTextAsync(testTargetFile.FullName, testContent, Encoding.UTF8);
            }
        }

        private readonly Options myOptions;
        private readonly Configuration myConfig;
        private readonly SolutionHandler mySolutionHandler;
    }
}
