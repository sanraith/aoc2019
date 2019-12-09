using System.Threading.Tasks;

namespace aoc2019.Puzzles.Core
{
    public interface IProgressPublisher
    {
        bool IsUpdateProgressNeeded();
        Task UpdateProgressAsync(double current, double total);
    }
}
