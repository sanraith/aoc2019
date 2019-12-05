using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Sunny with a Chance of Asteroids")]
    public sealed class Day05 : SolutionBase
    {
        public override string Part1(string input)
        {
            var program = ParseProgram(input);
            var outputStream = RunProgram(program, new[] { 1 });

            return outputStream.Last().ToString();
        }

        public override string Part2(string input)
        {
            var program = ParseProgram(input);
            var outputStream = RunProgram(program, new[] { 5 });

            return outputStream.Last().ToString();
        }

        private static IEnumerable<int> RunProgram(int[] memory, IEnumerable<int> inputStream)
        {
            int[] rawParams = new int[ParameterCountsByOpCode.Values.Max()];
            int[] parsedParams = new int[ParameterCountsByOpCode.Values.Max()];
            var inputEnumerator = inputStream.GetEnumerator();

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
                        memory[rawParams[0]] = inputEnumerator.MoveNext() ? inputEnumerator.Current :
                            throw new IndexOutOfRangeException("Not enough input values!");
                        break;
                    case 4:
                        yield return parsedParams[0];
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
                        yield break;
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

            var parameterCount = ParameterCountsByOpCode[opCode];
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

        private static int[] ParseProgram(string input) => GetLines(input).First().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();

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
    }
}
