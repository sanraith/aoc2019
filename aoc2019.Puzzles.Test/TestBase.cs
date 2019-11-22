using System;

namespace aoc2019.Puzzles.Test
{
    public abstract class TestBase<TSolution> where TSolution : ISolution
    {
        protected TSolution Solution { get; private set; }

        protected TestBase()
        {
            Solution = Activator.CreateInstance<TSolution>();
        }
    }
}
