namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// A safe, read-only view of an agent. 
/// Used by Hooks, UI, and Persistence layers.
/// </summary>
public interface IInspectableAgent
{
    /// <summary>
    /// Gets the persistable data representation.
    /// </summary>
    AgentData GetAgentData();
}
