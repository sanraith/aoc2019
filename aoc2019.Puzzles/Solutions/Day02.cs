using aoc2019.Puzzles.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("1202 Program Alarm")]
    public sealed class Day02 : SolutionBase
    {
        public override string Part1(string input)
        {
            return RunProgram(GetProgram(input), 12, 2).ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var originalProgram = GetProgram(input);
            var program = new int[originalProgram.Length];
            for (var noun = 0; noun < 100; noun++)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(noun, 100); }
                for (var verb = 0; verb < 100; verb++)
                {
                    originalProgram.CopyTo(program, 0);
                    var result = RunProgram(program, noun, verb);
                    if (result == 19690720)
                    {
                        return (100 * noun + verb).ToString();
                    }
                }
            }

            throw new InvalidOperationException("Could not find solution!");
        }

        private int RunProgram(int[] program, int noun, int verb)
        {
            program[1] = noun;
            program[2] = verb;

            var pos = 0;
            while (pos < program.Length && program[pos] != 99)
            {
                switch (program[pos])
                {
                    case 1:
                        program[program[pos + 3]] = program[program[pos + 1]] + program[program[pos + 2]];
                        break;
                    case 2:
                        program[program[pos + 3]] = program[program[pos + 1]] * program[program[pos + 2]];
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown operator: {program[pos]}");
                }
                pos += 4;
            }

            return program[0];
        }

        private static int[] GetProgram(string input) => GetLines(input).First().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
    }
}
