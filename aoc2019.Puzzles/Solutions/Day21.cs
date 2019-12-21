using aoc2019.Puzzles.Core;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day11;
using static aoc2019.Puzzles.Solutions.Day11.SynchronousIntMachine;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Springdroid Adventure")]
    public sealed class Day21 : SolutionBase
    {
        public bool IsInteractiveInput { get; set; } = false;
        public bool IsInteractiveOutput { get; set; } = false;

        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var autoInput = new[] {
                "OR A J",
                "AND B J",
                "AND C J",
                "NOT J J",
                "AND D J",
                "WALK" };
            var result = await RunAsciiMachine(intMachine, autoInput);

            return result.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var autoInput = new[] {
                "OR A J",
                "AND B J",
                "AND C J",
                "NOT J J",
                "AND D J",
                "NOT E T",
                "NOT T T",
                "OR H T",
                "AND T J",
                "RUN" };
            var result = await RunAsciiMachine(intMachine, autoInput);

            return result.ToString();
        }

        private async Task<long> RunAsciiMachine(SynchronousIntMachine intMachine, string[] inputLines)
        {
            if (!IsInteractiveInput)
            {
                inputLines.Select(l => l + '\n').SelectMany(l => l).ForEach(c => intMachine.InputQueue.Enqueue(c));
            }

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
                                if (IsInteractiveOutput) { Console.Write((char)value); }
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

        #region Generators for all possible input (did not help) 

        private void GetCombinations()
        {
            var registers = "ABCDEFGHI".ToArray();
            var values = new bool[registers.Length];
            var combinations = new Dictionary<string, bool?>();
            TryCombination(values, 0, combinations);

            Console.WriteLine("Truths:");
            Console.WriteLine(string.Join(",", combinations.Where(x => x.Value.HasValue && x.Value.Value).Select(x => $"\"{x.Key}\"")));
            Console.WriteLine();
            Console.WriteLine("Don't care:");
            Console.WriteLine(string.Join(",", combinations.Where(x => !x.Value.HasValue).Select(x => $"\"{x.Key}\"")));
            Console.WriteLine();
        }

        private int myCombinationIndex = 0;

        private void TryCombination(bool[] values, int index, Dictionary<string, bool?> combinations)
        {
            if (index == values.Length)
            {
                myCombinationIndex++;
                var combination = string.Join("", values.Select(x => x ? 1 : 0));
                var map = combination;
                var reachables = GetReachables(map);
                var canJump = reachables.Contains(3);
                var mustJump = !reachables.Contains(0);
                var reachablesIfNoJump = mustJump ? new List<int> { -1 } : GetReachables(new string(map.Skip(1).ToArray())).Select(x => x + 1).ToList();
                var reachablesIfJump = !canJump ? new List<int> { -1 } : GetReachables(new string(map.Skip(4).ToArray())).Select(x => x + 4).ToList();
                var shouldJump = mustJump || reachablesIfJump.Max() > reachablesIfNoJump.Max();

                if (mustJump && !canJump)
                {
                    combinations.Add(combination, null);
                }
                else
                {
                    combinations.Add(combination, shouldJump);
                }
                Console.WriteLine(myCombinationIndex + ". " + combination + ": " + combinations[combination]);
            }
            else
            {
                TryCombination(values, index + 1, combinations);
                values[index] = true;
                TryCombination(values, index + 1, combinations);
                values[index] = false;
            }
        }

        private List<int> GetReachables(string map)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>(new[] { -1 });
            var reachables = new List<int> { -1 };
            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                if (pos >= map.Length) { continue; }
                if (!visited.Add(pos)) { continue; }
                if (pos != -1)
                {
                    if (map[pos] == '1') { reachables.Add(pos); }
                    else { continue; }
                }

                queue.Enqueue(pos + 1);
                queue.Enqueue(pos + 4);
            }

            return reachables.OrderBy(x => x).ToList();
        }

        #endregion
    }
}
