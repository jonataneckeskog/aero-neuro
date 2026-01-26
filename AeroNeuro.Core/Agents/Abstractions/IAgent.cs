namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// The full agent capability used by the Trainer.
/// </summary>
public interface IAgent<T> : IInspectableAgent
{
    /// <summary>
    /// Decide on actions based on observations.
    /// </summary>
    T[] Decide(T[] observations);

    /// <summary>
    /// Create a deep copy of the agent.
    /// </summary>
    IAgent<T> Clone();

    /// <summary>
    /// Apply mutation to the agent's internal mutation system.
    /// </summary>
    void Mutate();
}
