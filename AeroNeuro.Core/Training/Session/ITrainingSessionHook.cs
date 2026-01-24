namespace AeroNeuro.Core.Training;

using AeroNeuro.Core.Environments;

/// <summary>
/// Represents a hook that is executed after each generation in the training session.
/// </summary>
public interface ITrainingSessionHook<T>
{
    /// <summary>
    /// Called after a generation has been evolved.
    /// </summary>
    /// <param name="stats">The statistics of the current generation.</param>
    /// <param name="trainer">The trainer instance (access to population/best agents).</param>
    /// <param name="environment">The environment instance (if available).</param>
    void OnGenerationEvolved(EvolutionStats stats, IEvolutionTrainer<T> trainer, IEnvironment<T>? environment);
}
