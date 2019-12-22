using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day22Test : TestBase<Day22>
    {
        [Test]
        public async Task Part1_Small1()
        {
            var input = @"cut 6
deal with increment 7
deal into new stack";
            Solution.CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.LastStack);
            Assert.That(stackString, Is.EqualTo("3 0 7 4 1 8 5 2 9 6"));
        }

        [Test]
        public async Task Part1_Small2()
        {
            var input = @"deal with increment 7
deal with increment 9
cut -2";
            Solution.CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.LastStack);
            Assert.That(stackString, Is.EqualTo("6 3 0 7 4 1 8 5 2 9"));
        }

        [Test]
        public async Task Part1_Large()
        {
            var input = @"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1";
            Solution.CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.LastStack);
            Assert.That(stackString, Is.EqualTo("9 2 5 8 1 4 7 0 3 6"));
        }


        //[Test]
        public async Task Part2()
        {
            var input = @"";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo(""));
        }
    }
}
