namespace AeroNeuro.Core.Training;

/// <summary>
/// Interface for displaying evolution training statistics.
/// </summary>
public interface IStatsDisplayer
{
    /// <summary>
    /// Displays the provided evolution statistics.
    /// </summary>
    /// <param name="stats"></param>
    void DisplayStats(EvolutionStats stats);
}