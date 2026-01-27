using AeroNeuro.Core.Common;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Session;

public class TrainingContext<T>
{
    private readonly IPopulationProvider<T> _populationProvider;
    private readonly IEvolutionStatsProvider _statsProvider;

    public TrainingContext(IPopulationProvider<T> populationProvider, IEvolutionStatsProvider statsProvider)
    {
        _populationProvider = populationProvider;
        _statsProvider = statsProvider;
    }

    /// <summary>
    /// Dynamically fetches the current population from the provider.
    /// </summary>
    public IEnumerable<(float Fitness, IInspectableAgent Agent)> Population => _populationProvider.GetInspectablePopulation();

    /// <summary>
    /// Critical stats from the evolution process.
    /// </summary>
    public EvolutionStats Stats => _statsProvider.GetCurrentStats();

    public bool ShouldStop { get; set; } = false;
}
