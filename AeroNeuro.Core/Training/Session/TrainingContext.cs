using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Session;

public class TrainingContext
{
    public TrainingContext(
        IReadOnlyList<IInspectableAgent> population,
        EvolutionStats stats)
    {
        Population = population;
        Stats = stats;
    }

    /// <summary>
    /// The population in a Read-Only, Safe-to-Inspect state.
    /// </summary>
    public IReadOnlyList<IInspectableAgent> Population { get; }

    /// <summary>
    /// Critical stats (Best Fitness, Generation Count, etc.)
    /// </summary>
    public EvolutionStats Stats { get; }

    /// <summary>
    /// A signal to the trainer to stop (e.g. if Target Fitness reached).
    /// </summary>
    public bool ShouldStop { get; set; } = false;
}