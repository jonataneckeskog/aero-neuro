namespace AeroNeuro.Core.Agents;

/// <summary>
/// Interface for an agent that can interact with an environment.
/// </summary>
public interface IAgent<T>
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

    /// <summary>
    /// Get the agent's data representation.
    /// </summary>
    AgentData GetAgentData();
}