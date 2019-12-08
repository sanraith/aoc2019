using aoc2019.Puzzles.Core;
using NUnit.Framework;
using System;

namespace aoc2019.Puzzles.Test
{
    public abstract class TestBase<TSolution> where TSolution : ISolution
    {
        protected TSolution Solution { get; private set; }

        [SetUp]
        protected virtual void SetUp()
        {
            Solution = Activator.CreateInstance<TSolution>();
        }
    }
}
