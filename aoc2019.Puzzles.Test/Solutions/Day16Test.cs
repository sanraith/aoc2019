using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day16Test : TestBase<Day16>
    {
        [Test]
        public async Task Part1()
        {
            var input = "80871224585914546619083218645595";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("24176176"));

            input = "19617804207202209144916044189917";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("73745418"));

            input = "69317163492948606335995924319873";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("52432133"));
        }

        //[Test]
        public async Task Part2()
        {
            var input = "03036732577212944063491565474664";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("84462026"));

            input = "02935109699940807407585447034323";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("78725270"));

            input = "03081770884921959731165446850517";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("53553731"));
        }
    }
}
