namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// Interface for providing new agent instances.
/// </summary>
public interface IAgentProvider<T>
{
    /// <summary>
    /// Creates a standard agent with the default starting complexity (e.g., 30 nodes).
    /// </summary>
    IAgent<T> CreateBaseAgent();

    /// <summary>
    /// Creates an agent with a randomized brain structure within a given complexity range.
    /// </summary>
    IAgent<T> CreateRandomAgent(int minNodes, int maxNodes);
}