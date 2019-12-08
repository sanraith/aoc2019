# Advent of Code 2019 C# Blazor solutions [![.NET Core build badge](https://github.com/sanraith/aoc2019/workflows/.NET%20Core/badge.svg)](https://github.com/sanraith/aoc2019/actions)

Solutions for [Advent of Code 2019](https://adventofcode.com/2019) in C# with a Blazor WebAssembly runner. This project uses .Net Core 3.1.  
Solutions can be run in console or directly inside a modern web browser, thanks to Blazor WebAssembly.

Try it at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/)

## Project structure

| Folder                 | Description
| ---                    | ---
| `aoc2019.ConsoleApp`   | Console application to prepare and run the puzzle solutions.
| `aoc2019.Puzzles`      | Inputs and solutions for the Advent of Code puzzles.
| `aoc2019.Puzzles.Test` | Unit tests for the solutions.
| `aoc2019.WebApp`       | Blazor WebAssembly application to run the puzzle solutions within a WebAssembly-compatible browser.
| `docs`                 | The published version of `aoc2019.WebApp`. Available at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/).

## Build and run

Make sure `.NET Core SDK 3.1.100-preview3-014645` or later is installed.  
Clone and build the solution:

- `git clone <https://github.com/sanraith/aoc2019>`
- `cd aoc2019`
- `dotnet build`

To run the Blazor WebAssembly application:

- `dotnet run -p aoc2019.WebApp`
- Open `<http://localhost:52016/>`

To run all puzzle solutions in console:

- `dotnet run -p aoc2019.ConsoleApp --all`

To run the last solution in console:

- `dotnet run -p aoc2019.ConsoleApp --last`

To run a specific solution in console:

- `dotnet run -p aoc2019.ConsoleApp --day `**`[number of day]`**

To setup the environment for a new puzzle solution:

- Set your [adventofcode.com](adventofcode.com) session cookie for `aoc2019.ConsoleApp` as a user secret:
  - `dotnet user-secrets -p aoc2019.ConsoleApp set "SessionCookie" `**`"Your session cookie"`**
- Run setup to create source, test, input and description files for the given day:
  - `dotnet run -p aoc2019.ConsoleApp --setup `**`[number of day]`**
