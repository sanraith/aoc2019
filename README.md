# Advent of Code 2019 C# Blazor solutions [![](https://github.com/sanraith/aoc2019/workflows/.NET%20Core/badge.svg)](https://github.com/sanraith/aoc2019/actions)
Solutions for [Advent of Code 2019](https://adventofcode.com/2019) in C# with a Blazor WebAssembly runner.  
Try it at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/)

# Project structure
| Folder    						| Description
| ---								| ---
| <code>aoc2019.ConsoleApp</code>	| Console application to prepare and run the puzzle solutions.
| <code>aoc2019.Puzzles</code>		| Inputs and solutions for the Advent of Code puzzles.
| <code>aoc2019.Puzzles.Test</code>	| Unit tests for the solutions.
| <code>aoc2019.WebApp</code>		| Blazor WebAssembly application to run the puzzle solutions within a WebAssembly-compatible browser.
| <code>docs</code>					| The published version of <code>aoc2019.WebApp</code>. Available at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/).

# Build and run
Make sure <code>.NET Core SDK 3.1.100-preview3-014645</code> or later is installed.  
- <code>dotnet build</code>
- Run the Blazor WebAssembly application:
    - <code>dotnet run --project aoc2019.WebApp</code>
    - Open <code>http://localhost:52016/</code>
- Run the last puzzle solution in console:
    - <code>dotnet run --project aoc2019.ConsoleApp --last</code>
- Run a puzzle solution in console:
    - <code>dotnet run --project aoc2019.ConsoleApp --day **[number of day]**</code>
- Create source, test, input and description files for a puzzle:
    - Set your [adventofcode.com]() session cookie in <code>aoc2019.ConsoleApp\\appsettings.json\\SessionCookie</code>, or as a user secret.
    - <code>dotnet run --project aoc2019.ConsoleApp --setup **[number of day]**</code>
