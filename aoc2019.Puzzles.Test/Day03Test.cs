using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test
{
    public sealed class Day03Test : TestBase<Day03>
    {
        [Test]
        public async Task Part1()
        {
            var input = @"R8,U5,L5,D3
U7,R6,D4,L4";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("6"));

            input = @"R75,D30,R83,U83,L12,D49,R71,U7,L72
U62,R66,U55,R34,D71,R55,D58,R83";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("159"));

            input = @"R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51
U98,R91,D20,R16,D67,R40,U7,R15,U6,R7";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("135"));
        }

        [Test]
        public async Task Part2()
        {
            var input = @"R8,U5,L5,D3
U7,R6,D4,L4";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("30"));

            input = @"R75,D30,R83,U83,L12,D49,R71,U7,L72
U62,R66,U55,R34,D71,R55,D58,R83";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("610"));

            input = @"R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51
U98,R91,D20,R16,D67,R40,U7,R15,U6,R7";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("410"));
        }
    }
}
