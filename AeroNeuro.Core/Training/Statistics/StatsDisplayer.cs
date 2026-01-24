namespace AeroNeuro.Core.Training;

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