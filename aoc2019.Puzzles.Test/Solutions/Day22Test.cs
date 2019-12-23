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
            Solution.Part1CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.Part1LastStack);
            Assert.That(stackString, Is.EqualTo("3 0 7 4 1 8 5 2 9 6"));
        }

        [Test]
        public async Task Part1_Small2()
        {
            var input = @"deal with increment 7
deal with increment 9
cut -2";
            Solution.Part1CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.Part1LastStack);
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
            Solution.Part1CardCount = 10;
            _ = await Solution.Part1Async(input);
            var stackString = string.Join(" ", Solution.Part1LastStack);
            Assert.That(stackString, Is.EqualTo("9 2 5 8 1 4 7 0 3 6"));
        }

        [Test]
        public async Task Part2()
        {
            Assert.That(await Solution.Part2Async(myInput), Is.EqualTo("71345377301237"));
        }

        private readonly string myInput = @"deal into new stack
deal with increment 32
cut 5214
deal with increment 50
cut -7078
deal with increment 3
cut 5720
deal with increment 18
cut -6750
deal with increment 74
cut -6007
deal with increment 16
cut -3885
deal with increment 40
deal into new stack
cut -2142
deal with increment 25
deal into new stack
cut -1348
deal with increment 40
cut 3943
deal with increment 14
cut 7093
deal with increment 67
cut 1217
deal with increment 75
cut 597
deal with increment 60
cut -1078
deal with increment 68
cut -8345
deal with increment 25
cut 6856
deal into new stack
cut -4152
deal with increment 59
deal into new stack
cut -80
deal with increment 3
deal into new stack
deal with increment 44
cut 1498
deal with increment 18
cut -7149
deal with increment 58
deal into new stack
deal with increment 71
cut -323
deal into new stack
deal with increment 58
cut 1793
deal with increment 45
deal into new stack
cut 7187
deal with increment 48
cut 2664
deal into new stack
cut 8943
deal with increment 32
deal into new stack
deal with increment 62
cut -9436
deal with increment 67
deal into new stack
cut -1898
deal with increment 61
deal into new stack
deal with increment 14
cut 1287
deal with increment 8
cut 560
deal with increment 6
cut -2110
deal with increment 8
cut 9501
deal with increment 25
cut 4791
deal with increment 70
deal into new stack
deal with increment 5
cut 2320
deal with increment 47
cut -467
deal into new stack
deal with increment 19
cut -1920
deal with increment 16
cut -8920
deal with increment 65
cut -3986
deal with increment 3
cut -2690
deal with increment 35
cut -757
deal with increment 37
cut -1280
deal with increment 71
cut 3765
deal with increment 26
deal into new stack";
    }
}
