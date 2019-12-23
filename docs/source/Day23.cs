using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day09;
using static aoc2019.Puzzles.Solutions.Day11;
using static aoc2019.Puzzles.Solutions.Day11.SynchronousIntMachine;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Category Six")]
    public sealed class Day23 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            SetupMachines(input, out var machines, out var queues);

            while (true)
            {
                foreach (var machine in machines)
                {
                    foreach (var (address, _, y) in HandleOutgoingPackets(MachineCount, queues, machine))
                    {
                        if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
                        if (address > MachineCount)
                        {
                            return y.ToString();
                        }
                    }
                }
                HandleIncomingPackets(machines, queues);
            }
        }

        public override async Task<string> Part2Async(string input)
        {
            SetupMachines(input, out var machines, out var queues);

            (long X, long Y) natValue = default;
            long? lastNatDeliveredY = null;
            while (true)
            {
                var idleCount = 0;
                foreach (var machine in machines)
                {
                    if (machine.InputQueue.Count == 1 && machine.InputQueue.Single() == -1)
                    {
                        idleCount++;
                    }

                    foreach (var (address, x, y) in HandleOutgoingPackets(MachineCount, queues, machine))
                    {
                        if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
                        if (address > MachineCount)
                        {
                            natValue = (x, y);
                        }
                    }
                }

                if (idleCount == MachineCount)
                {
                    if (natValue.Y == lastNatDeliveredY) { break; }
                    queues[0].Enqueue(natValue.X);
                    queues[0].Enqueue(natValue.Y);
                    lastNatDeliveredY = natValue.Y;
                }

                HandleIncomingPackets(machines, queues);
            }

            return lastNatDeliveredY.ToString();
        }

        private IEnumerable<(int Address, long X, long Y)> HandleOutgoingPackets(int machineCount, Queue<long>[] queues, SynchronousIntMachine machine)
        {
            while (machine.RunUntilBlockOrComplete() == ReturnCode.WrittenOutput)
            {
                var packet = ReadPacket(machine);
                var (address, x, y) = packet;
                if (address < machineCount)
                {
                    queues[address].Enqueue(x);
                    queues[address].Enqueue(y);
                }
                yield return packet;
            }
        }

        private static void HandleIncomingPackets(SynchronousIntMachine[] machines, Queue<long>[] queues)
        {
            foreach (var (machine, index) in machines.WithIndex())
            {
                var inputQueue = queues[index];
                if (inputQueue.Count == 0)
                {
                    machine.InputQueue.Enqueue(-1);
                }
                else
                {
                    while (inputQueue.Count > 0) { machine.InputQueue.Enqueue(inputQueue.Dequeue()); }
                }
            }
        }

        private static (int Address, long X, long Y) ReadPacket(SynchronousIntMachine machine)
        {
            var o = machine.OutputQueue;
            while (o.Count < 3) { machine.RunUntilBlockOrComplete(); }

            return ((int)o.Dequeue(), o.Dequeue(), o.Dequeue());
        }

        private static void SetupMachines(string input, out SynchronousIntMachine[] machines, out Queue<long>[] queues)
        {
            var memory = IntMachineBase.ParseProgram(input);
            machines = Enumerable.Range(0, MachineCount).Select(_ => new SynchronousIntMachine(memory.ToArray())).ToArray();
            queues = Enumerable.Range(0, MachineCount).Select(_ => new Queue<long>()).ToArray();
            machines.WithIndex().ForEach(m => m.Item.InputQueue.Enqueue(m.Index));
        }

        private const int MachineCount = 50;
    }
}
