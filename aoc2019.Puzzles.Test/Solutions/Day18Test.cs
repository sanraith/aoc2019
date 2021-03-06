﻿using aoc2019.Puzzles.Solutions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace aoc2019.Puzzles.Test.Solutions
{
    public sealed class Day18Test : TestBase<Day18>
    {
        [Test]
        public async Task Part1_Small()
        {
            var input = @"#########
#b.A.@.a#
#########";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("8"));
        }

        [Test]
        public async Task Part1_Medium()
        {
            var input = @"########################
#f.D.E.e.C.b.A.@.a.B.c.#
######################.#
#d.....................#
########################";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("86"));
        }

        [Test]
        public async Task Part1_Medium2()
        {
            var input = @"########################
#...............b.C.D.f#
#.######################
#.....@.a.B.c.d.A.e.F.g#
########################";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("132"));
        }

        [Test]
        public async Task Part1_Medium3()
        {
            var input = @"########################
#@..............ac.GI.b#
###d#e#f################
###A#B#C################
###g#h#i################
########################";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("81"));
        }

        [Test]
        public async Task Part1_Large()
        {
            var input = @"#################
#i.G..c...e..H.p#
########.########
#j.A..b...f..D.o#
########@########
#k.E..a...g..B.n#
########.########
#l.F..d...h..C.m#
#################";
            Assert.That(await Solution.Part1Async(input), Is.EqualTo("136"));
        }

        [Test]
        public async Task Part2_Medium()
        {
            var input = @"###############
#d.ABC.#.....a#
######...######
######.@.######
######...######
#b.....#.....c#
###############";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("24"));
        }

        [Test]
        public async Task Part2_Medium2()
        {
            var input = @"#############
#DcBa.#.GhKl#
#.###...#I###
#e#d#.@.#j#k#
###C#...###J#
#fEbA.#.FgHi#
#############";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("32"));
        }

        [Test]
        public async Task Part2_Medium3()
        {
            var input = @"#############
#g#f.D#..h#l#
#F###e#E###.#
#dCba...BcIJ#
#####.@.#####
#nK.L...G...#
#M###N#H###.#
#o#m..#i#jk.#
#############";
            Assert.That(await Solution.Part2Async(input), Is.EqualTo("72"));
        }
    }
}
