namespace AeroNeuro.Core.Common;

/// <summary>
/// Interface for providing agents with context from the evolution.
/// </summary>
public interface IEvolutionStatsProvider
{
    /// <summary>
    /// Gets the EvolutionStats from the in-progress evolution.
    /// </summary>
    /// <returns></returns>
    EvolutionStats GetCurrentStats();

    /// <summary>
    /// Updates the EvolutionStats.
    /// </summary>
    /// <param name="newStats"></param>
    void Update(EvolutionStats newStats);
}