using System;

namespace aoc2019.Puzzles
{
    public sealed class SolutionMetadata
    {
        public int Day { get; }

        public Type Type { get; }

        public string Title { get; }

        public SolutionMetadata(Type type, int day, string name)
        {
            Type = type;
            Day = day;
            Title = name;
        }

        public ISolution CreateInstance()
        {
            return (ISolution)Activator.CreateInstance(Type);
        }
    }
}
