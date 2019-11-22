using aoc2019.Puzzles.Solutions;
using NUnit.Framework;

namespace aoc2019.Puzzles.Test
{
    [TestFixture]
    public sealed class Day01Test : TestBase<Day01>
    {
        [Test]
        public void Part1()
        {
            Assert.That(Solution.Part1("hello"), Is.Not.Null);
        }

        [Test]
        public void Part2()
        {
            Assert.That(Solution.Part2("hello"), Is.Not.Null);
        }
    }
}