using aoc2019.WebApp.Visualizers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc2019.WebApp.Services
{
    public interface IVisualizerHandler
    {
        IVisualizer GetVisualizer(Type solutionType);
    }

    public sealed class VisualizerHandler : IVisualizerHandler
    {
        public IReadOnlyDictionary<Type, Type> VisualizersBySolutionType { get; }

        public VisualizerHandler()
        {
            VisualizersBySolutionType = GatherPuzzleSolutions();
        }

        public IVisualizer GetVisualizer(Type solutionType)
        {
            if (VisualizersBySolutionType.TryGetValue(solutionType, out var visualizerType))
            {
                return (IVisualizer)Activator.CreateInstance(visualizerType);
            }
            return null;
        }

        private static Dictionary<Type, Type> GatherPuzzleSolutions()
        {
            var visualizersBySolutionType = new Dictionary<Type, Type>();
            var visualizerInterface = typeof(IVisualizer);
            var visualizerTypes = visualizerInterface.Assembly.GetTypes()
                .Where(x => visualizerInterface.IsAssignableFrom(x) && !x.IsAbstract)
                .ToList();

            foreach (var visualizerType in visualizerTypes)
            {
                var targetSolutionAttributes = visualizerType.GetCustomAttributes(typeof(TargetSolutionAttribute), false).OfType<TargetSolutionAttribute>().ToList();
                foreach (var targetSolutionType in targetSolutionAttributes.Select(x => x.TargetSolutionType))
                {
                    visualizersBySolutionType[targetSolutionType] = visualizerType;
                }
            }

            return visualizersBySolutionType;
        }
    }
}
