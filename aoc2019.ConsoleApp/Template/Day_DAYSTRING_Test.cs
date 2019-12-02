using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test
{
    public sealed class Day_DAYSTRING_Test : TestBase<Day_DAYSTRING_>
    {
        [Test]
        public async Task Part1()
        {
            Assert.That(await Solution.Part1Async(""), Is.Not.Null);
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2Async(""), Is.Not.Null);
        }
    }
}
