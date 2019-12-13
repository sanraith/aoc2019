using aoc2019.Puzzles.Core;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Visualizers
{
    public interface IVisualizer
    {
        ISolution SolutionInstance { get; set; }
    }
}
