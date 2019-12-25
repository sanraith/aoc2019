using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day11;
using static aoc2019.Puzzles.Solutions.Day11.SynchronousIntMachine;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Cryostasis")]
    public sealed class Day25 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var result = await RunAsciiMachine(intMachine);

            return result.ToString();
        }

        public override string Part2(string input)
        {
            return "";
        }

        private async Task<long> RunAsciiMachine(SynchronousIntMachine intMachine)
        {
            ReturnCode returnCode;
            while ((returnCode = intMachine.RunUntilBlockOrComplete()) != ReturnCode.Completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
                switch (returnCode)
                {
                    case ReturnCode.WaitingForInput:
                        var userInput = Console.ReadLine() + '\n';
                        userInput.ToList().ForEach(x => intMachine.InputQueue.Enqueue(x));
                        break;
                    case ReturnCode.WrittenOutput:
                        while (intMachine.OutputQueue.Count > 0)
                        {
                            var value = intMachine.OutputQueue.Dequeue();
                            if (value < 256)
                            {
                                Console.Write((char)value);
                            }
                            else
                            {
                                return value;
                            }
                        }
                        break;
                }
            }
            return -1;
        }
    }
}
