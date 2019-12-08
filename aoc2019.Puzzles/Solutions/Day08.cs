using aoc2019.Puzzles.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc2019.Puzzles.Solutions
{
    [Puzzle("Space Image Format")]
    public sealed class Day08 : SolutionBase
    {
        public int Width { get; set; } = 25;
        public int Height { get; set; } = 6;
        public char BlackChar { get; set; } = ' ';
        public char WhiteChar { get; set; } = '#';

        public override string Part1(string input)
        {
            var layers = GetLayers(input);
            var minZeroLayer = layers.OrderBy(l => l.Count(c => c == '0')).First();
            var hash = minZeroLayer.Count(c => c == '1') * minZeroLayer.Count(c => c == '2');

            return hash.ToString();
        }

        public override string Part2(string input)
        {
            var layers = GetLayers(input);
            var layerLength = Height * Width;
            var imageSource = new string('3', layerLength).ToArray();

            foreach (var layer in layers.Reverse<string>())
            {
                for (var i = 0; i < layerLength; i++)
                {
                    var newPixel = layer[i];
                    if (newPixel != '2') { imageSource[i] = newPixel; }
                }
            }
            var image = new string(imageSource).Replace('0', BlackChar).Replace('1', WhiteChar);
            image = string.Join(Environment.NewLine, Enumerable.Repeat(Width, Height).Select((w, i) => image.Substring(i * w, w)));

            return image;
        }

        private List<string> GetLayers(string input)
        {
            var line = GetLines(input).First();
            var length = line.Length;
            var layerLength = Height * Width;
            var layers = new List<string>();

            var pos = 0;
            while (pos < length)
            {
                layers.Add(line.Substring(pos, layerLength));
                pos += layerLength;
            }

            return layers;
        }
    }
}
