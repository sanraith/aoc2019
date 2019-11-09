# Advent of Code 2019 solutions
Solutions for [Advent of Code 2019](https://adventofcode.com/2019) with a Blazor WebAssembly runner.  
Try it at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/)

# Project structure
| Folder    					| Description
| ---							| ---
| <code>aoc2019.Puzzles</code>	| Inputs and solutions for the Advent of Code puzzles.
| <code>aoc2019.WebApp</code>	| Blazor WebAssembly application to run the puzzle solutions within a WebAssembly-compatible browser.
| <code>docs</code>				| The published version of <code>aoc2019.WebApp</code>. Available at: [https://sanraith.github.io/aoc2019/](https://sanraith.github.io/aoc2019/).

# Build and run
Make sure <code>.NET Core SDK 3.1.100-preview2-014569</code> or later is installed.  
- <code>dotnet build</code>
- <code>dotnet run --project aoc2019.WebApp</code>
- Open <code>http://localhost:52016/</code>
