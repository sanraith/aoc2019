using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Services
{
    public interface IInputHandler
    {
        Task<string> GetInputAsync(int day);

        object[] GetResults(int day);

        void ClearResults(int day);
    }

    public sealed class InputHandler : IInputHandler
    {
        public InputHandler(HttpClient httpClient)
        {
            myHttpClient = httpClient;
        }

        public async Task<string> GetInputAsync(int day)
        {
            if (!myInputCache.TryGetValue(day, out var input))
            {
                var dayString = day.ToString().PadLeft(2, '0');
                input = await myHttpClient.GetStringAsync($"input/day{dayString}.txt");
                myInputCache.Add(day, input);
            }

            return input;
        }

        public object[] GetResults(int day)
        {
            if (!myResultCache.TryGetValue(day, out var results))
            {
                results = new object[2];
                myResultCache.Add(day, results);
            }
            return results;
        }

        public void ClearResults(int day)
        {
            var results = GetResults(day);
            for (var i = 0; i < results.Length; i++)
            {
                results[i] = null;
            }
        }

        private readonly HttpClient myHttpClient;
        private readonly Dictionary<int, object[]> myResultCache = new Dictionary<int, object[]>();
        private readonly Dictionary<int, string> myInputCache = new Dictionary<int, string>();
    }
}
