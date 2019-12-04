using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day04Test : TestBase<Day04>
    {
        [Test]
        public async Task Part1_SingleValue()
        {
            Assert.That(await Solution.Part1Async("111111-111111"), Is.EqualTo("1"));
            Assert.That(await Solution.Part1Async("223450-223450"), Is.EqualTo("0"));
            Assert.That(await Solution.Part1Async("123789-123789"), Is.EqualTo("0"));
        }

        [Test]
        public async Task Part1_PuzzleInput()
        {
            Assert.That(await Solution.Part1Async(myPuzzleInput), Is.EqualTo("925"));
        }

        [Test]
        public async Task Part2_SingleValue()
        {
            Assert.That(await Solution.Part2Async("112233-112233"), Is.EqualTo("1"));
            Assert.That(await Solution.Part2Async("123444-123444"), Is.EqualTo("0"));
            Assert.That(await Solution.Part2Async("111122-111122"), Is.EqualTo("1"));
        }

        [Test]
        public async Task Part2_PuzzleInput()
        {
            Assert.That(await Solution.Part2Async(myPuzzleInput), Is.EqualTo("607"));
        }

        private readonly string myPuzzleInput = "271973-785961";
    }
}
