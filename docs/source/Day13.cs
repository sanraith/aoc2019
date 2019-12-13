using aoc2019.Puzzles.Core;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day09;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day11;
using static aoc2019.Puzzles.Solutions.Day11.SynchronousIntMachine;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Care Package")]
    public sealed class Day13 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var intMachine = new SynchronousIntMachine(input);
            var tiles = await LoadTiles(intMachine);
            int blockCount = tiles.SelectMany(x => x).Count(x => x == Tile.Block);

            return blockCount.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var memory = IntMachineBase.ParseProgram(input);
            memory[0] = 2; // Insert Coin
            var intMachine = new SynchronousIntMachine(memory);
            var tiles = await LoadTiles(intMachine);
            int maxBlockCount = tiles.SelectMany(x => x).Count(x => x == Tile.Block);

            long score = 0;
            var blockCount = maxBlockCount;
            var ball = Point.Empty;
            var paddle = Point.Empty;
            ReturnCode returnCode;
            while (blockCount > -1 && (returnCode = intMachine.RunUntilBlockOrComplete()) != ReturnCode.Completed)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(maxBlockCount - blockCount, maxBlockCount); }

                switch (returnCode)
                {
                    case ReturnCode.WaitingForInput:
                        if (blockCount == 0) { blockCount = -1; break; }
                        var joystickInput = ball.X.CompareTo(paddle.X);
                        intMachine.InputQueue.Enqueue(joystickInput);
                        break;
                    case ReturnCode.WrittenOutput:
                        HandleTileChange(intMachine, tiles, ref score, ref blockCount, ref ball, ref paddle);
                        break;
                }
            }

            return score.ToString();
        }

        private void HandleTileChange(SynchronousIntMachine intMachine, Tile[][] tiles, ref long score, ref int blockCount, ref Point ball, ref Point paddle)
        {
            var (x, y, t) = GetTile(intMachine);
            if (x == -1)
            {
                score = t;
            }
            else
            {
                var tile = (Tile)t;
                if (tile != Tile.Block && tiles[x][y] == Tile.Block) { blockCount--; }
                tiles[x][y] = tile;

                if (tile == Tile.Ball)
                {
                    ball = new Point(x, y);
                }
                else if (tile == Tile.Paddle)
                {
                    paddle = new Point(x, y);
                }
            }
        }

        private async Task<Tile[][]> LoadTiles(SynchronousIntMachine intMachine)
        {
            var tilesDict = new Dictionary<Point, Tile>();
            while (intMachine.RunUntilBlockOrComplete() == ReturnCode.WrittenOutput)
            {
                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(); }
                var (x, y, tile) = GetTile(intMachine);
                tilesDict[new Point(x, y)] = (Tile)tile;
            }

            var width = tilesDict.Keys.Max(p => p.X) + 1;
            var height = tilesDict.Keys.Max(p => p.Y) + 1;
            var tiles = Enumerable.Range(0, width).Select(x => new Tile[height]).ToArray();
            tilesDict.Where(t => t.Key.X >= 0).ForEach(t => tiles[t.Key.X][t.Key.Y] = t.Value);

            return tiles;
        }

        private (int x, int y, long t) GetTile(SynchronousIntMachine intMachine)
        {
            while (intMachine.OutputQueue.Count < 3) { intMachine.RunUntilBlockOrComplete(); }
            var x = (int)intMachine.OutputQueue.Dequeue();
            var y = (int)intMachine.OutputQueue.Dequeue();
            var tile = intMachine.OutputQueue.Dequeue();

            return (x, y, tile);
        }

        private enum Tile
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            Paddle = 3,
            Ball = 4
        }
    }
}
