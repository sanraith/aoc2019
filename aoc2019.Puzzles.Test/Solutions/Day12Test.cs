using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day12Test : TestBase<Day12>
    {
        [Test]
        public async Task Part1_10Steps()
        {
            Solution.Part1StepCount = 10;
            var input = @"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("179"));
        }

        [Test]
        public async Task Part1_100Steps()
        {
            Solution.Part1StepCount = 100;
            var input = @"<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("1940"));
        }

        //[Test]
        //public async Task Part2()
        //{
        //    var input = @"";
        //    Assert.That(await Solution.Part2Async(input), Is.EqualTo(""));
        //}
    }
}
