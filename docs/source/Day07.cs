using aoc2019.Puzzles.Core;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Amplification Circuit")]
    public sealed class Day07 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var memory = IntMachine.ParseProgram(input);
            int maxAmplified = await GetMaxAmplifiedValue(memory, new[] { 0, 1, 2, 3, 4 });

            return maxAmplified.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var memory = IntMachine.ParseProgram(input);
            int maxAmplified = await GetMaxAmplifiedValue(memory, new[] { 5, 6, 7, 8, 9 });

            return maxAmplified.ToString();
        }

        private async Task<int> GetMaxAmplifiedValue(IReadOnlyCollection<int> originalMemory, IEnumerable<int> possiblePhases)
        {
            var phaseSequences = possiblePhases.Permutations().ToList();
            var maxAmplified = int.MinValue;
            foreach (var (phaseSequence, index) in phaseSequences.Select((x, i) => (x, i)))
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(index, phaseSequences.Count); }

                var output = await RunAmplifiers(originalMemory, phaseSequence);
                maxAmplified = Math.Max(maxAmplified, output);
            }

            return maxAmplified;
        }

        private static async Task<int> RunAmplifiers(IReadOnlyCollection<int> originalMemory, IList<int> phaseSequence)
        {
            var sequenceLength = phaseSequence.Count;

            // Create channels
            var channels = new Channel<int>[sequenceLength];
            foreach (var (phase, phaseIndex) in phaseSequence.Select((x, i) => (x, i)))
            {
                var channel = Channel.CreateUnbounded<int>();
                await channel.Writer.WriteAsync(phase);
                channels[phaseIndex] = channel;
            }
            await channels[0].Writer.WriteAsync(0);

            // Create amplifiers
            var amplifiers = new IntMachine[sequenceLength];
            for (int i = 0; i < sequenceLength; i++)
            {
                var inputChannel = channels[i];
                var outputChannel = channels[(i + 1) % sequenceLength];
                amplifiers[i] = new IntMachine(originalMemory.ToArray(), inputChannel, outputChannel);
            }

            // Run amplifiers
            await Task.WhenAll(amplifiers.Select(x => x.RunProgram()));
            var result = await amplifiers.Last().OutputChannel.Reader.ReadAsync();
            return result;
        }

        private sealed class IntMachine
        {
            public Channel<int> InputChannel { get; }

            public Channel<int> OutputChannel { get; }

            public IntMachine(int[] memory, Channel<int> inputChannel = null, Channel<int> outputChannel = null)
            {
                myMemory = memory;
                InputChannel = inputChannel ?? Channel.CreateUnbounded<int>();
                OutputChannel = outputChannel ?? Channel.CreateUnbounded<int>();
            }

            public static int[] ParseProgram(string input) => GetLines(input).First().Split(new[] { ',' }).Select(x => Convert.ToInt32(x)).ToArray();

            public Task RunProgram() => RunProgram(myMemory, InputChannel.Reader, OutputChannel.Writer);

            private async Task RunProgram(int[] memory, ChannelReader<int> inputChannel, ChannelWriter<int> outputChannel)
            {
                int[] rawParams = new int[ParameterCountsByOpCode.Values.Max()];
                int[] parsedParams = new int[ParameterCountsByOpCode.Values.Max()];

                int pos = 0;
                while (pos < memory.Length)
                {
                    var (opCode, parameterModes) = ParseInstruction(memory[pos]);
                    ParseParams(memory, pos, parameterModes, ref rawParams, ref parsedParams);

                    switch (opCode)
                    {
                        case 1:
                            memory[rawParams[2]] = parsedParams[0] + parsedParams[1];
                            break;
                        case 2:
                            memory[rawParams[2]] = parsedParams[0] * parsedParams[1];
                            break;
                        case 3:
                            memory[rawParams[0]] = await inputChannel.ReadAsync();
                            break;
                        case 4:
                            await outputChannel.WriteAsync(parsedParams[0]);
                            break;
                        case 5:
                            if (parsedParams[0] != 0) { pos = parsedParams[1]; continue; }
                            break;
                        case 6:
                            if (parsedParams[0] == 0) { pos = parsedParams[1]; continue; }
                            break;
                        case 7:
                            memory[rawParams[2]] = parsedParams[0] < parsedParams[1] ? 1 : 0;
                            break;
                        case 8:
                            memory[rawParams[2]] = parsedParams[0] == parsedParams[1] ? 1 : 0;
                            break;
                        case 99:
                            return;
                        default:
                            throw new InvalidOperationException($"Unknown operator: {opCode}");
                    }

                    pos += parameterModes.Length + 1;
                }
            }

            private static (int OpCode, int[] ParameterModes) ParseInstruction(int instruction)
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

            private static void ParseParams(int[] memory, int opPos, int[] parameterModes, ref int[] rawParams, ref int[] parsedParams)
            {
                var count = parameterModes.Length;
                Array.Copy(memory, opPos + 1, rawParams, 0, count);
                for (var i = 0; i < count; i++)
                {
                    parsedParams[i] = parameterModes[i] == 0 ? memory[rawParams[i]] : rawParams[i];
                }
            }

            private static readonly Dictionary<int, int> ParameterCountsByOpCode = new Dictionary<int, int>
            {
                [1] = 3,
                [2] = 3,
                [3] = 1,
                [4] = 1,
                [5] = 2,
                [6] = 2,
                [7] = 3,
                [8] = 3,
                [99] = 0
            };

            private readonly int[] myMemory;
        }
    }
}
