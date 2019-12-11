using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day09Test : TestBase<Day09>
    {
        [Test]
        public async Task Part1_IntMachine_Quine()
        {
            var input = @"109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var intMachine = new Day09.IntMachine(input);
            await intMachine.RunProgramAsync();
            var outputs = new List<long>();
            await foreach (var output in intMachine.OutputChannel.ReadAllAsync()) { outputs.Add(output); }
            Assert.That(outputs, Is.EqualTo(input.Split(',').Select(x => Convert.ToInt64(x)).ToList()));
        }

        [Test]
        public async Task Part1_IntMachine_LargeNumbers()
        {
            var input = @"1102,34915192,34915192,7,4,7,99,0";
            var outputs = new List<long>();
            var intMachine = new Day09.IntMachine(input);
            await intMachine.RunProgramAsync();
            await foreach (var output in intMachine.OutputChannel.ReadAllAsync())
            {
                outputs.Add(output);
            }
            Assert.That(outputs.Single().ToString().Length, Is.EqualTo(16));

            input = @"104,1125899906842624,99";
            outputs = new List<long>();
            intMachine = new Day09.IntMachine(input);
            await intMachine.RunProgramAsync();
            await foreach (var output in intMachine.OutputChannel.ReadAllAsync()) { outputs.Add(output); }
            Assert.That(outputs.Single(), Is.EqualTo(1125899906842624));
        }
    }
}
