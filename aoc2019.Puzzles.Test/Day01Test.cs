using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test
{
    public sealed class Day01Test : TestBase<Day01>
    {
        [Test]
        public async Task Part1()
        {
            Assert.That(await Solution.Part1("12"), Is.EqualTo("2"));
            Assert.That(await Solution.Part1("14"), Is.EqualTo("2"));
            Assert.That(await Solution.Part1("1969"), Is.EqualTo("654"));
            Assert.That(await Solution.Part1("100756"), Is.EqualTo("33583"));
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2("12"), Is.EqualTo("2"));
            Assert.That(await Solution.Part2("1969"), Is.EqualTo("966"));
            Assert.That(await Solution.Part2("100756"), Is.EqualTo("50346"));
        }
    }
}
