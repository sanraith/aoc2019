using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test
{
    [TestFixture]
    public sealed class Day91Test : TestBase<Day91>
    {
        [Test]
        public async Task Part1()
        {
            Assert.That(await Solution.Part1("hello"), Is.Not.Null);
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2("hello"), Is.Not.Null);
        }
    }
}