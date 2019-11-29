using aoc2019.Puzzles.Solutions;
using NUnit.Framework;

namespace aoc2019.Puzzles.Test
{
    public sealed class Day_DAYSTRING_Test : TestBase<Day_DAYSTRING_>
    {
        [Test]
        public void Part1()
        {
            Assert.That(Solution.Part1(""), Is.Not.Null);
        }

        [Test]
        public void Part2()
        {
            Assert.That(Solution.Part2(""), Is.Not.Null);
        }
    }
}
