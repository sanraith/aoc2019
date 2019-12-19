using aoc2019.Puzzles.Core;
using aoc2019.Puzzles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static aoc2019.Puzzles.Solutions.Day09;
using static aoc2019.Puzzles.Solutions.Day10;
using static aoc2019.Puzzles.Solutions.Day11;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Tractor Beam")]
    public sealed class Day19 : SolutionBase
    {
        public override async Task<string> Part1Async(string input)
        {
            var memory = IntMachineBase.ParseProgram(input);
            var sum = 0;
            Console.WriteLine();
            for (var y = 0; y < 50; y++)
            {
                for (var x = 0; x < 50; x++)
                {
                    if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(x + y * 50, 50 * 50); }

                    var isPulling = IsPulling(memory, x, y);
                    sum += isPulling ? 1 : 0;
                    Console.Write(isPulling ? '#' : '.');
                }
                Console.WriteLine();
            }

            return sum.ToString();
        }

        public override async Task<string> Part2Async(string input)
        {
            var stepRight = new Point(1, 0);
            var stepDown = new Point(0, 1);
            var memory = IntMachineBase.ParseProgram(input);
            var startTop = GetStartPoint(memory);
            var startBottom = startTop;
            var margins = new List<(Point Top, Point Bottom)>();

            var size = 100 - 1;
            Point targetPoint;

            while (true)
            {
                var top = GetTop(memory, startTop);
                var bottom = GetBottom(memory, startBottom);
                margins.Add((top, bottom));
                startTop = top + stepRight;
                startBottom = bottom + stepRight + stepDown;

                if (IsUpdateProgressNeeded()) { await UpdateProgressAsync(bottom.Y - top.Y, size * 2); }

                var currentMarginIndex = margins.Count - 1;
                if (currentMarginIndex >= size && margins[currentMarginIndex - size].Bottom.Y >= top.Y + size)
                {
                    targetPoint = new Point(margins[currentMarginIndex - size].Top.X, top.Y);
                    break;
                }
            }

            return (targetPoint.X * 10000 + targetPoint.Y).ToString();
        }

        private static Point GetTop(long[] memory, Point startPoint)
        {
            var top = startPoint.Y;

            if (IsPulling(memory, startPoint.X, top))
            {
                while (IsPulling(memory, startPoint.X, top)) { top--; }
                top++;
            }
            else
            {
                while (!IsPulling(memory, startPoint.X, top)) { top++; }
            }


            return new Point(startPoint.X, top);
        }

        private static Point GetBottom(long[] memory, Point startPoint)
        {
            var bottom = startPoint.Y;

            if (IsPulling(memory, startPoint.X, bottom))
            {
                while (IsPulling(memory, startPoint.X, bottom)) { bottom++; }
                bottom--;
            }
            else
            {
                while (!IsPulling(memory, startPoint.X, bottom)) { bottom--; }
            }


            return new Point(startPoint.X, bottom);
        }

        //private enum Margin { Top, Bottom };

        //private static Point GetMargin(long[] memory, Point startPoint, Margin vertical)
        //{
        //    var margin = startPoint.Y;

        //    if (IsPulling(memory, startPoint.X, margin) ^ vertical != Margin.Top)
        //    {
        //        while (IsPulling(memory, startPoint.X, margin)) { margin--; }
        //        margin++;
        //    }
        //    else
        //    {
        //        while (!IsPulling(memory, startPoint.X, margin)) { margin++; }
        //    }


        //    return new Point(startPoint.X, margin);
        //}

        private static Point GetStartPoint(long[] memory)
        {
            for (var y = 1; y < 10; y++)
            {
                for (var x = 1; x < 10; x++)
                {
                    if (IsPulling(memory, x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }
            throw new IndexOutOfRangeException();
        }

        private static bool IsPulling(long[] memory, int x, int y)
        {
            var intMachine = new SynchronousIntMachine(memory);
            intMachine.InputQueue.Enqueue(x);
            intMachine.InputQueue.Enqueue(y);
            intMachine.RunUntilBlockOrComplete();
            return intMachine.OutputQueue.Dequeue() == 1;
        }
    }
}
