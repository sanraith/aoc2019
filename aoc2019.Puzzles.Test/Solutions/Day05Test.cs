using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day05Test : TestBase<Day05>
    {
        [Test]
        public async Task Part1_InputOutput()
        {
            var input = "3,0,4,0,99";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("1"));
        }

        [Test]
        public async Task Part1_ParameterModes()
        {
            var input = "1002,7,3,7,4,7,99,33";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("99"));
        }

        [Test]
        public async Task Part1_NegativeNumbers()
        {
            var input = "1101,100,-89,7,4,7,99,0";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("11"));
        }

        [Test]
        public async Task Part2_CompareOperators()
        {
            var input = "3,9,8,9,10,9,4,9,99,-1,8";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("0"));
            
            input = "3,9,7,9,10,9,4,9,99,-1,8";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("1"));

            input = "3,3,1108,-1,8,3,4,3,99";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("0"));

            input = "3,3,1107,-1,8,3,4,3,99";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("1"));
        }

        [Test]
        public async Task Part2_JumpOperators()
        {
            var input = "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("1"));

            input = "3,3,1105,-1,9,1101,0,0,12,4,12,99,1";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("1"));

            input = "3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("999"));
        }
    }
}
