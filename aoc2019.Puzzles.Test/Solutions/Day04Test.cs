using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day04Test : TestBase<Day04>
    {
        [Test]
        public async Task Part1()
        {
            Assert.That(await Solution.Part1Async(myInput), Is.EqualTo("925"));
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2Async(myInput), Is.EqualTo("607"));
        }

        private readonly string myInput = "271973-785961";
    }
}
