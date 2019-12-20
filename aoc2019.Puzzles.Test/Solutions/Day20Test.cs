using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day20Test : TestBase<Day20>
    {
        [Test]
        public async Task Part1()
        {
            var input = @"         A           
         A           
  #######.#########  
  #######.........#  
  #######.#######.#  
  #######.#######.#  
  #######.#######.#  
  #####  B    ###.#  
BC...##  C    ###.#  
  ##.##       ###.#  
  ##...DE  F  ###.#  
  #####    G  ###.#  
  #########.#####.#  
DE..#######...###.#  
  #.#########.###.#  
FG..#########.....#  
  ###########.#####  
             Z       
             Z       ";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("23"));
        }

        [Test]
        public async Task Part2()
        {
            var input = @"";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo(""));
        }
    }
}
