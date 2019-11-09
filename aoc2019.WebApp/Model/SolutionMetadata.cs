using System;

namespace aoc2019.WebApp.Model
{
    public sealed class SolutionMetadata
    {
        public int Day { get; }

        public Type Type { get; }

        public string Name { get; }

        public string Description { get; }

        public SolutionMetadata(Type type, int day, string name, string description = null)
        {
            Type = type;
            Day = day;
            Name = name;
            Description = description;
        }
    }
}
