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
            Assert.That(await Solution.Part1Async("12"), Is.EqualTo("2"));
            Assert.That(await Solution.Part1Async("14"), Is.EqualTo("2"));
            Assert.That(await Solution.Part1Async("1969"), Is.EqualTo("654"));
            Assert.That(await Solution.Part1Async("100756"), Is.EqualTo("33583"));
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2Async("12"), Is.EqualTo("2"));
            Assert.That(await Solution.Part2Async("1969"), Is.EqualTo("966"));
            Assert.That(await Solution.Part2Async("100756"), Is.EqualTo("50346"));
        }
    }
}
