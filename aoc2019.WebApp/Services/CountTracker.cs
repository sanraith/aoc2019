using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Services
{
    public interface ICountTracker
    {
        int Value { get; set; }
    }

    public class CountTracker : ICountTracker
    {
        public int Value { get; set; }
    }
}
