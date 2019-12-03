using System;

namespace aoc2019.WebApp.Visualizers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class TargetSolutionAttribute : Attribute
    {
        public Type TargetSolutionType { get; }
        
        public TargetSolutionAttribute(Type targetSolutionType)
        {
            TargetSolutionType = targetSolutionType;
        }
    }
}
