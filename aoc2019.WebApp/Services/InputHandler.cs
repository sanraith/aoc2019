using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace aoc2019.WebApp.Services
{
    public interface IInputHandler
    {
        Task<string> GetInputAsync(int day);
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

        private readonly HttpClient myHttpClient;
        private readonly Dictionary<int, string> myInputCache = new Dictionary<int, string>();
    }
}
