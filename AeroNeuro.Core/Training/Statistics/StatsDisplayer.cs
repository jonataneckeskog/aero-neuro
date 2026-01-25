using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Statistics;

public class StatsDisplayer : IStatsDisplayer
{
    private readonly Action<EvolutionStats> _displayStrategy;

    public StatsDisplayer(Action<EvolutionStats> displayStrategy)
    {
        _displayStrategy = displayStrategy;
    }

    /// <inheritdoc/>
    public void DisplayStats(EvolutionStats stats)
    {
        _displayStrategy(stats);
    }
}
