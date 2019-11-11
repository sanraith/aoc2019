using aoc2019.Puzzles;
using aoc2019.WebApp.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace aoc2019.WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICountTracker, CountTracker>();
            services.AddSingleton<ISolutionHandler, SolutionHandler>();
            services.AddSingleton<IInputHandler, InputHandler>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
