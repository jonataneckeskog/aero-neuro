using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Abstractions;

/// <summary>
/// Interface for an evolution trainer that manages a population of agents,
/// evaluates them in an environment and selects the best performers.
/// </summary>
public interface IEvolutionTrainer<T>
{
    /// <summary>
    /// Run one generation of evolution.
    /// Evaluates population, selects best, creates mutants.
    /// </summary>
    void EvolveGeneration();

    /// <summary>
    /// Get the current best agents in the population.
    /// </summary>
    List<IAgent<T>> GetBestAgents(int count);

    /// <summary>
    /// Get statistics about current generation.
    /// </summary>
    EvolutionStats GetStats();
}
