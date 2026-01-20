namespace AeroNeuro.Core.Agents;

/// <summary>
/// Interface for providing new agent instances.
/// </summary>
public interface IAgentProvider
{
    /// <summary>
    /// Creates a standard agent with the default starting complexity (e.g., 30 nodes).
    /// </summary>
    IAgent CreateBaseAgent();

    /// <summary>
    /// Creates an agent with a randomized brain structure within a given complexity range.
    /// </summary>
    IAgent CreateRandomAgent(int minNodes, int maxNodes);
}