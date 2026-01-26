namespace AeroNeuro.Core.Common;

public class EvolutionStatsProvider : IEvolutionStatsProvider
{
    private EvolutionStats _currentStats;

    public EvolutionStatsProvider()
    {
        _currentStats = new EvolutionStats(0, 0f, 0f, 0f, TimeSpan.FromTicks(0));
    }

    /// <inheritdoc/>
    public EvolutionStats GetCurrentStats() => _currentStats;


    /// <inheritdoc/>
    public void Update(EvolutionStats newStats) => _currentStats = newStats;
}
