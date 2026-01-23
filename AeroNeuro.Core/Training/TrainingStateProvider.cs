using AeroNeuro.Core.Agents;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Concrete implementation of ITrainingStateProvider that tracks and provides training state.
/// </summary>
public class TrainingStateProvider : ITrainingStateProvider
{
    private EvolutionStats _currentStats;

    public TrainingStateProvider()
    {
        _currentStats = new EvolutionStats(0, 0f, 0f, 0f, TimeSpan.Zero);
    }

    public TrainingStateProvider(EvolutionStats initialStats)
    {
        _currentStats = initialStats;
    }

    /// <summary>
    /// Updates the training state with new statistics.
    /// </summary>
    public void UpdateStats(EvolutionStats stats)
    {
        _currentStats = stats;
    }

    /// <inheritdoc/>
    public int GetGenerationCount() => _currentStats.Generation;

    /// <inheritdoc/>
    public float GetBestFitness() => _currentStats.BestFitness;

    /// <inheritdoc/>
    public float GetAverageFitness() => _currentStats.AverageFitness;

    /// <inheritdoc/>
    public float GetWorstFitness() => _currentStats.WorstFitness;

    /// <inheritdoc/>
    public TimeSpan GetGenerationDuration() => _currentStats.GenerationDuration;
}
