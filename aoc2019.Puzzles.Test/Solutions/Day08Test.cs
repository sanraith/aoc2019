using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day08Test : TestBase<Day08>
    {
        [Test]
        public async Task Part1()
        {
            var input = "123456789012";
            Solution.Width = 3;
            Solution.Height = 2;

            Assert.That(await Solution.Part1Async(input), Is.EqualTo("1"));
        }

        [Test]
        public async Task Part2()
        {
            var input = "0222112222120000";
            var expectedResult = $"01{Environment.NewLine}10";
            Solution.Width = 2;
            Solution.Height = 2;
            Solution.BlackChar = '0';
            Solution.WhiteChar = '1';

            Assert.That(await Solution.Part2Async(input), Is.EqualTo(expectedResult));
        }
    }
}
