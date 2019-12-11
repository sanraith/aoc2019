using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntMachineBase = aoc2019.Puzzles.Solutions.Day09.IntMachineBase;
using Point = aoc2019.Puzzles.Solutions.Day10.Point;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Space Police")]
    public sealed class Day11 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var canvas = new Dictionary<Point, int>();
            var paintedPositionCount = await Paint(intMachine, canvas);

            return paintedPositionCount.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var canvas = new Dictionary<Point, int> { [new Point(0, 0)] = 1 };
            await Paint(intMachine, canvas);

            return Render(canvas);
        }

        private static string Render(Dictionary<Point, int> canvas)
        {
            var whitePoints = canvas.Where(x => x.Value == 1).Select(x => x.Key).ToList();
            var topLeft = new Point(whitePoints.Min(x => x.X), whitePoints.Min(x => x.Y));
            var bottomRight = new Point(whitePoints.Max(x => x.X), whitePoints.Max(x => x.Y));
            var resultSb = new StringBuilder();
            for (var x = topLeft.X; x <= bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y <= bottomRight.Y; y++)
                {
                    if (canvas.TryGetValue(new Point(x, y), out var color))
                    {
                        resultSb.Append(color == 0 ? ' ' : '#');
                    }
                    else
                    {
                        resultSb.Append(' ');
                    }
                }
                resultSb.AppendLine();
            }

            return resultSb.ToString();
        }

        private async Task<int> Paint(SynchronousIntMachine intMachine, Dictionary<Point, int> canvas)
        {
            var paintedPositions = new HashSet<Point>();
            var directions = new[] { new Point(-1, 0), new Point(0, 1), new Point(1, 0), new Point(0, -1) };
            var direction = 0;
            var pos = new Point(0, 0);

            while (intMachine.RunUntilBlockOrComplete() != SynchronousIntMachine.ReturnCode.Completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }

                var color = canvas.GetOrAdd(pos, _ => 0);
                intMachine.InputQueue.Enqueue(color);

                intMachine.RunUntilBlockOrComplete();
                canvas[pos] = (int)intMachine.OutputQueue.Dequeue();
                paintedPositions.Add(pos);

                intMachine.RunUntilBlockOrComplete();
                var directionDelta = (int)intMachine.OutputQueue.Dequeue() == 0 ? -1 : 1;
                direction = (direction + directionDelta + 4) % 4;
                pos += directions[direction];
            }

            return paintedPositions.Count;
        }

        public sealed class SynchronousIntMachine : IntMachineBase
        {
            public enum ReturnCode
            {
                Error = -1,
                WaitingForInput = 3,
                WrittenOutput = 4,
                Completed = 99
            }

            public Queue<long> InputQueue { get; }

            public Queue<long> OutputQueue { get; }

            public SynchronousIntMachine(string input, Queue<long> inputQueue = null, Queue<long> outputQueue = null) : this(ParseProgram(input), inputQueue, outputQueue)
            { }

            public SynchronousIntMachine(long[] memory, Queue<long> inputQueue = null, Queue<long> outputQueue = null) : base(memory)
            {
                InputQueue = inputQueue ?? new Queue<long>();
                OutputQueue = outputQueue ?? new Queue<long>();
            }

            public ReturnCode RunUntilBlockOrComplete()
            {
                var rawParams = new long[ParameterCountsByOpCode.Values.Max()];
                var resolvedParams = new long[ParameterCountsByOpCode.Values.Max()];

                ReturnCode? returnCode = null;
                while (myPos >= 0 && returnCode == null)
                {
                    var (opCode, parameterModes) = ParseInstruction((int)myMemory[myPos]);
                    ResolveParams(myPos, parameterModes, ref rawParams, ref resolvedParams);

                    switch (opCode)
                    {
                        case 1:
                            myMemory[rawParams[2]] = resolvedParams[0] + resolvedParams[1];
                            break;
                        case 2:
                            myMemory[rawParams[2]] = resolvedParams[0] * resolvedParams[1];
                            break;
                        case 3:
                            if (InputQueue.Count == 0) { return ReturnCode.WaitingForInput; } // Read cannot be completed, return and repeat the same command on continue.
                            myMemory[rawParams[0]] = InputQueue.Dequeue();
                            break;
                        case 4:
                            OutputQueue.Enqueue(resolvedParams[0]);
                            returnCode = ReturnCode.WrittenOutput;
                            break;
                        case 5:
                            if (resolvedParams[0] != 0) { myPos = resolvedParams[1]; continue; }
                            break;
                        case 6:
                            if (resolvedParams[0] == 0) { myPos = resolvedParams[1]; continue; }
                            break;
                        case 7:
                            myMemory[rawParams[2]] = resolvedParams[0] < resolvedParams[1] ? 1 : 0;
                            break;
                        case 8:
                            myMemory[rawParams[2]] = resolvedParams[0] == resolvedParams[1] ? 1 : 0;
                            break;
                        case 9:
                            myRelativeBase += resolvedParams[0];
                            break;
                        case 99:
                            returnCode = ReturnCode.Completed;
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown operator: {opCode}");
                    }

                    myPos += parameterModes.Length + 1;
                }

                return returnCode ?? ReturnCode.Error;
            }

            private long myPos = 0;
        }
    }
}
