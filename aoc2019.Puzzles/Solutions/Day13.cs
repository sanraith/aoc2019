using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IntMachine = aoc2019.Puzzles.Solutions.Day09.IntMachine;
using Point = aoc2019.Puzzles.Solutions.Day10.Point;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Care Package")]
    public sealed class Day13 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var tiles = await LoadTiles(input);
            int blockCount = tiles.Values.Count(x => x == Tile.Block);

            return blockCount.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var tiles = await LoadTiles(input);

            throw new NotImplementedException();
        }

        private async Task<Dictionary<Point, Tile>> LoadTiles(string input)
        {
            var intMachine = new IntMachine(input) { ProgressPublisher = this };
            _ = intMachine.RunProgramAsync();

            var tiles = new Dictionary<Point, Tile>();
            while (await intMachine.OutputChannel.WaitToReadAsync())
            {
                var x = (int)await intMachine.OutputChannel.ReadAsync();
                var y = (int)await intMachine.OutputChannel.ReadAsync();
                var tile = (Tile)await intMachine.OutputChannel.ReadAsync();
                tiles[new Point(x, y)] = tile;
            }

            return tiles;
        }

        private enum Tile
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HorizontalPaddle = 3,
            Ball = 4
        }
    }
}
