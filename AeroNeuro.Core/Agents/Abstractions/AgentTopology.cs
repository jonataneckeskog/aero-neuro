namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// Describes the input and output size of an agent.
/// </summary>
/// <param name="InputCount"></param>
/// <param name="OutputCount"></param>
public record AgentTopology(int InputCount, int OutputCount);