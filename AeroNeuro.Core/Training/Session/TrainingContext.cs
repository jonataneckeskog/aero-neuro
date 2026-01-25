using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// Contains the state and context for the current training session.
/// Passed to hooks during lifecycle events.
/// </summary>
public class TrainingContext<T>
{
    public TrainingContext(IEvolutionTrainer<T> trainer, IEnvironment<T>? environment = null)
    {
        Trainer = trainer;
        Environment = environment;
    }

    /// <summary>
    /// The trainer instance managing the population.
    /// </summary>
    public IEvolutionTrainer<T> Trainer { get; }

    /// <summary>
    /// The environment being used for training (if any).
    /// </summary>
    public IEnvironment<T>? Environment { get; }

    /// <summary>
    /// The statistics of the most recent generation. 
    /// May be null if called before the first generation evolves (e.g. OnSessionStart).
    /// </summary>
    public EvolutionStats? Stats { get; set; }

    /// <summary>
    /// Set to true by a hook to signal the session to stop early.
    /// </summary>
    public bool ShouldStop { get; set; } = false;
}
