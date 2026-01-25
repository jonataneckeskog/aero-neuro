namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// Basic type for each agent. Mainly used to save agents to disk
/// for persistance.
/// </summary>
public abstract class AgentData
{
    public abstract string Type { get; }
}
