using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using MoreLinq;
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
            var map = await GetMap(intMachine);
            var width = map.Keys.Max(p => p.X + 1);
            var height = map.Keys.Max(p => p.Y + 1);

            var alignmentSum = 0;
            for (var y = 1; y < height - 1; y++)
            {
                for (var x = 1; x < width - 1; x++)
                {
                    if (map[new Point(x, y)] == '#' &&
                        map[new Point(x - 1, y)] == '#' &&
                        map[new Point(x + 1, y)] == '#' &&
                        map[new Point(x, y + 1)] == '#' &&
                        map[new Point(x, y - 1)] == '#')
                    {
                        alignmentSum += x * y;
                    }
                }
            }

            return alignmentSum.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var memory = IntMachineBase.ParseProgram(input);
            memory[0] = 2; // Wake up
            var intMachine = new SynchronousIntMachine(memory);
            var map = await GetMap(intMachine);

            await UpdateProgressAsync(.5, 1);

            var (robotPos, robotChar) = map.First(x => RobotDirections.Contains(x.Value));
            var robotDirection = Directions[Array.IndexOf(RobotDirections, robotChar)];
            map[robotPos] = Scaffolding;

            myGarbageCounter = char.MaxValue;
            var path = await FollowPath(map, robotPos, robotDirection);
            var commandData = await FindSlicing(path.ToArray());
            var commands = commandData.Select(x => ConvertCommand(x.Command)).ToArray();
            var movementRoutine = CreateMovementRoutine(commandData);

            movementRoutine.ForEach(x => intMachine.InputQueue.Enqueue(x));
            commands.SelectMany(x => x).ForEach(x => intMachine.InputQueue.Enqueue(x));
            "n\n".ForEach(x => intMachine.InputQueue.Enqueue(x));

            while (intMachine.RunUntilBlockOrComplete() != ReturnCode.Completed) { }

            return intMachine.OutputQueue.Last().ToString();
        }

        private char[] ConvertCommand(char[] commandSource)
        {
            return commandSource
                .SelectMany(x => (x > 10000 ? ((char)(x - 10000)).ToString() : ((int)x).ToString()) + ",")
                .SkipLast(1)
                .Concat(new[] { '\n' })
                .ToArray();
        }

        private char[] CreateMovementRoutine(List<(char[] Command, int[] Indexes)> commandData)
        {
            return commandData
                .SelectMany((c, commandId) =>
                    c.Indexes.Select(i => (CommandId: commandId, Index: i)))
                .OrderBy(x => x.Index)
                .SelectMany(x => new[] { (char)(65 + x.CommandId), ',' })
                .SkipLast(1)
                .Concat(new[] { '\n' })
                .ToArray();
        }

        private async Task<List<(char[] Command, int[] Indexes)>> FindSlicing(char[] sequenceArray, int commandCount = 0, int garbagedOriginal = 0)
        {
            if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

            var sequence = sequenceArray.AsMemory();
            var sequenceLength = sequence.Length;
            for (var length = Math.Min(10, sequenceLength / 2); length > 1; length--)
            {
                for (var start = 0; start < sequenceLength - length * 2; start++)
                {
                    var proposed = sequence.Slice(start, length);
                    if (MemoryExtensions.Contains(sequence.Slice(start + length).Span, proposed.Span, StringComparison.Ordinal))
                    {
                        var repeating = proposed.ToArray();
                        var newSequence = sequence.ToArray();
                        var (garbageCount, indexes) = ReplaceWithGarbage(newSequence, repeating);
                        if (garbageCount + garbagedOriginal == sequenceLength)
                        {
                            return new List<(char[], int[])> { (repeating, indexes) };
                        }
                        else if (commandCount < 2)
                        {
                            var result = await FindSlicing(newSequence, commandCount + 1, garbagedOriginal + garbageCount);
                            if (result != null)
                            {
                                result.Add((repeating, indexes));
                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private (int GarbageCount, int[] Indexes) ReplaceWithGarbage(char[] sequenceArray, char[] repeatingArray)
        {
            var garbageCount = 0;
            var sequence = sequenceArray.AsSpan();
            var repeating = repeatingArray.AsSpan();

            int index = 0;
            var indexes = new List<int>();
            int deltaIndex;
            while ((deltaIndex = MemoryExtensions.IndexOf(sequence.Slice(index), repeating, StringComparison.Ordinal)) >= 0)
            {
                index += deltaIndex;
                indexes.Add(index);
                for (var i = index; i < index + repeating.Length; i++)
                {
                    sequence[i] = (char)myGarbageCounter;
                    myGarbageCounter--;
                    garbageCount++;
                }
                index += repeating.Length;
            }
            return (garbageCount, indexes.ToArray());
        }

        private async Task<List<char>> FollowPath(Dictionary<Point, char> map, Point startPos, Point startDirection)
        {
            var turnCommands = new Dictionary<int, char> { [1] = 'R', [-1] = 'L' }; 
            var path = new List<char>();
            var pos = startPos;
            var directionIndex = Array.IndexOf(Directions, startDirection);
            var partLength = 0;

            while (directionIndex < 4)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

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
                foreach (var (turnDelta, turnCommand) in turnCommands)
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
            var east = Directions[1];
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
                    map.Add(pos, (char)c);
                    pos += east;
                }
            }

            return map;
        }

        private int myGarbageCounter = char.MaxValue;

        private readonly Point[] Directions = new[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
        private readonly char[] RobotDirections = new[] { '^', '>', 'v', '<' };

        private const char Scaffolding = '#';
    }
}
