using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day24Test : TestBase<Day24>
    {
        [Test]
        public async Task Part1()
        {
            var input = @"....#
#..#.
#..##
..#..
#....";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("2129920"));
        }

        [Test]
        public async Task Part2()
        {
            var input = @"....#
#..#.
#.?##
..#..
#....";
            Solution.Part2MinuteCount = 10;
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("99"));
        }
    }
}
