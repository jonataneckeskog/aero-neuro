namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// Interface for providing agents with context from the evolution.
/// </summary>
public interface IEvolutionContext
{
    /// <summary>
    /// Gets the EvolutionStats from the in-progress evolution.
    /// </summary>
    /// <returns></returns>
    EvolutionStats GetCurrentStats();
}