using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day06Test : TestBase<Day06>
    {
        [Test]
        public async Task Part1()
        {
            var input = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("42"));
        }

        [Test]
        public async Task Part2()
        {
            var input = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L
K)YOU
I)SAN";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("4"));
        }
    }
}
