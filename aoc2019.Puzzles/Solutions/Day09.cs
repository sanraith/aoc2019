using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Sensor Boost")]
    public sealed class Day09 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new IntMachine(input) { ProgressPublisher = this };
            await intMachine.InputChannel.WriteAsync(1);
            await intMachine.RunProgramAsync();

            var result = await intMachine.OutputChannel.ReadAsync();
            return result.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var intMachine = new IntMachine(input) { ProgressPublisher = this };
            await intMachine.InputChannel.WriteAsync(2);
            await intMachine.RunProgramAsync();

            var result = await intMachine.OutputChannel.ReadAsync();
            return result.ToString();
        }

        public sealed class IntMachine : IntMachineBase
        {
            public IProgressPublisher ProgressPublisher { get; set; }

            public ChannelWriter<long> InputChannel => myInputChannel.Writer;

            public ChannelReader<long> OutputChannel => myOutputChannel.Reader;

            public IntMachine(string programString, Channel<long> inputChannel = null, Channel<long> outputChannel = null)
                : this(ParseProgram(programString), inputChannel, outputChannel) { }

            public IntMachine(long[] memory, Channel<long> inputChannel = null, Channel<long> outputChannel = null) : base(memory)
            {
                myInputChannel = inputChannel ?? Channel.CreateUnbounded<long>();
                myOutputChannel = outputChannel ?? Channel.CreateUnbounded<long>();
            }

            public Task RunProgramAsync() => RunProgramAsync(myInputChannel.Reader, myOutputChannel.Writer);

            private async Task RunProgramAsync(ChannelReader<long> InputChannel, ChannelWriter<long> OutputChannel)
            {
                var rawParams = new long[ParameterCountsByOpCode.Values.Max()];
                var resolvedParams = new long[ParameterCountsByOpCode.Values.Max()];

                long pos = 0;
                while (pos >= 0)
                {
                    if (ProgressPublisher != null && ProgressPublisher.IsUpdateProgressNeeded()) { await ProgressPublisher.UpdateProgressAsync(0, 1); }

                    var (opCode, parameterModes) = ParseInstruction((int)myMemory[pos]);
                    ResolveParams(pos, parameterModes, ref rawParams, ref resolvedParams);

                    switch (opCode)
                    {
                        case 1:
                            myMemory[rawParams[2]] = resolvedParams[0] + resolvedParams[1];
                            break;
                        case 2:
                            myMemory[rawParams[2]] = resolvedParams[0] * resolvedParams[1];
                            break;
                        case 3:
                            myMemory[rawParams[0]] = await InputChannel.ReadAsync();
                            break;
                        case 4:
                            await OutputChannel.WriteAsync(resolvedParams[0]);
                            break;
                        case 5:
                            if (resolvedParams[0] != 0) { pos = resolvedParams[1]; continue; }
                            break;
                        case 6:
                            if (resolvedParams[0] == 0) { pos = resolvedParams[1]; continue; }
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
                            OutputChannel.Complete();
                            return;
                        default:
                            throw new InvalidOperationException($"Unknown operator: {opCode}");
                    }

                    pos += parameterModes.Length + 1;
                }
            }

            private readonly Channel<long> myInputChannel;
            private readonly Channel<long> myOutputChannel;
        }

        public abstract class IntMachineBase
        {
            protected IntMachineBase(long[] memory)
            {
                myMemory = memory.Select((v, i) => (v, i)).ToDictionary(x => (long)x.i, x => x.v);
            }

            public static long[] ParseProgram(string input) => GetLines(input).First().Split(new[] { ',' }).Select(x => Convert.ToInt64(x)).ToArray();

            protected static (int OpCode, int[] ParameterModes) ParseInstruction(int instruction)
            {
                var opCode = instruction % 100;
                instruction /= 100;

                if (!ParameterCountsByOpCode.TryGetValue(opCode, out var parameterCount))
                {
                    throw new InvalidOperationException($"Unknown operator: {opCode}");
                }
                var parameterModes = new int[parameterCount];
                var index = 0;
                while (instruction > 0)
                {
                    parameterModes[index++] = instruction % 10;
                    instruction /= 10;
                }

                return (opCode, parameterModes);
            }

            protected void ResolveParams(long opPos, int[] parameterModes, ref long[] rawParams, ref long[] resolvedParams)
            {
                var count = parameterModes.Length;
                for (var i = 0; i < count; i++)
                {
                    rawParams[i] = myMemory[opPos + 1 + i];
                }

                for (var i = 0; i < count; i++)
                {
                    switch (parameterModes[i])
                    {
                        case 0:
                            resolvedParams[i] = myMemory.GetOrAdd(rawParams[i], _ => 0);
                            break;
                        case 1:
                            resolvedParams[i] = rawParams[i];
                            break;
                        case 2:
                            resolvedParams[i] = myMemory.GetOrAdd(rawParams[i] + myRelativeBase, _ => 0);
                            rawParams[i] = rawParams[i] + myRelativeBase;
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown parameter mode: {parameterModes[i]}");
                    }
                }
            }

            protected static readonly Dictionary<int, int> ParameterCountsByOpCode = new Dictionary<int, int>
            {
                [1] = 3,
                [2] = 3,
                [3] = 1,
                [4] = 1,
                [5] = 2,
                [6] = 2,
                [7] = 3,
                [8] = 3,
                [9] = 1,
                [99] = 0
            };

            protected long myRelativeBase = 0;
            protected readonly Dictionary<long, long> myMemory;
        }
    }
}
