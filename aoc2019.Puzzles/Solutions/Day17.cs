using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day09;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day11;
using static aoc2019.Puzzles.Solutions.Day11.SynchronousIntMachine;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Set and Forget")]
    public sealed class Day17 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);

            var lines = new List<char[]>();
            var line = new List<char>();
            while (intMachine.RunUntilBlockOrComplete() != SynchronousIntMachine.ReturnCode.Completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                var c = intMachine.OutputQueue.Dequeue();
                if (c == 10 && line.Any())
                {
                    lines.Add(line.ToArray());
                    line = new List<char>();
                }
                else
                {
                    line.Add((char)c);
                }
            }

            var width = lines.First().Length;
            var height = lines.Count;
            var alignmentSum = 0;
            for (var y = 1; y < height - 1; y++)
            {
                for (var x = 1; x < width - 1; x++)
                {
                    if (lines[y][x] == '#' &&
                        lines[y - 1][x] == '#' &&
                        lines[y + 1][x] == '#' &&
                        lines[y][x + 1] == '#' &&
                        lines[y][x - 1] == '#')
                    {
                        alignmentSum += x * y;
                    }
                }
            }

            // Draw the map
            Console.WriteLine();
            lines.ForEach(l => Console.WriteLine(new string(l)));

            return alignmentSum.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var memory = IntMachineBase.ParseProgram(input);
            memory[0] = 2; // Wake up
            var intMachine = new SynchronousIntMachine(memory);
            var map = await GetMap(intMachine);


            var (robotPos, robotChar) = map.First(x => RobotDirections.Contains(x.Value));
            var robotDirection = Directions[Array.IndexOf(RobotDirections, robotChar)];
            map[robotPos] = Scaffolding;

            var path = FollowPath(map, robotPos, robotDirection);
            Fuck(path.ToArray());

            return "";
        }

        private void Fuck(char[] sequenceArray)
        {
            var originalArray = sequenceArray.ToArray();
            var garbage = 65535;
            var sequence = sequenceArray.AsSpan();
            var repeateds = new List<char[]>();
            var garbaged = 0;
            while (true)
            {
                var repeating = FindLargestRepeatingSubSequence(sequenceArray);
                var index = 0;
                int deltaIndex;
                while ((deltaIndex = MemoryExtensions.IndexOf(sequence.Slice(index), repeating, StringComparison.Ordinal)) >= 0)
                {
                    index += deltaIndex;
                    for (var i = index; i < index + repeating.Length; i++)
                    {
                        sequence[i] = (char)garbage;
                        garbage--;
                        garbaged++;
                    }
                    index += repeating.Length;
                }
                repeateds.Add(repeating);
            }
        }

        private char[] FindLargestRepeatingSubSequence(char[] sequenceArray)
        {
            var sequence = sequenceArray.AsSpan();
            var sequenceLength = sequence.Length;
            char[] repeating;
            for (var length = Math.Min(10, sequenceLength / 2); length > 1; length--)
            {
                for (var start = 0; start < sequenceLength - length * 2; start++)
                {
                    var proposed = sequence.Slice(start, length);
                    if (MemoryExtensions.Contains(sequence.Slice(start + length), proposed, StringComparison.Ordinal))
                    {
                        repeating = proposed.ToArray();
                        return repeating;
                    }
                }
            }
            return null;
        }

        private List<char> FollowPath(Dictionary<Point, char> map, Point startPos, Point startDirection)
        {
            var path = new List<char>();
            var pos = startPos;
            var directionIndex = Array.IndexOf(Directions, startDirection);
            var partLength = 0;

            while (directionIndex < 4)
            {
                var nextPos = pos + Directions[directionIndex];
                if (map.TryGetValue(nextPos, out var tile) && tile == Scaffolding)
                {
                    partLength++;
                    pos = nextPos;
                    continue;
                }

                if (partLength > 0)
                {
                    path.Add((char)partLength);
                    partLength = 0;
                }

                directionIndex += 4;
                foreach (var (turnDelta, turnCommand) in TurnCommands)
                {
                    var proposedDirectionIndex = (directionIndex + turnDelta + 4) % 4;
                    var proposedPos = pos + Directions[proposedDirectionIndex];
                    if (map.TryGetValue(proposedPos, out tile) && tile == Scaffolding)
                    {
                        directionIndex = proposedDirectionIndex;
                        path.Add((char)(turnCommand + 10000));
                        break;
                    }
                }
            }

            return path;
        }

        private async Task<Dictionary<Point, char>> GetMap(SynchronousIntMachine intMachine)
        {
            var east = new Point(1, 0);
            var map = new Dictionary<Point, char>();
            var pos = new Point(0, 0);
            while (intMachine.RunUntilBlockOrComplete() == ReturnCode.WrittenOutput)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                var c = intMachine.OutputQueue.Dequeue();
                if (c == 10)
                {
                    pos = new Point(0, pos.Y + 1);
                }
                else
                {
                    if ((char)c != '.') { map.Add(pos, (char)c); }
                    pos += east;
                }
            }

            return map;
        }

        private readonly Point[] Directions = new[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
        private readonly char[] RobotDirections = new[] { '^', '>', 'v', '<' };
        private readonly Dictionary<int, char> TurnCommands = new Dictionary<int, char> { [1] = 'R', [-1] = 'L' };
        private const char Scaffolding = '#';
    }
}
