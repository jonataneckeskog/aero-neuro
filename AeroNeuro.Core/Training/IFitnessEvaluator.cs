using AeroNeuro.Core.Agents;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Interface for evaluating the fitness of agents. Higher fitness indicates better performance.
/// </summary>
public interface IFitnessEvaluator
{
    /// <summary>
    /// Evaluate fitness of an agent. Higher = better.
    /// </summary>
    float Evaluate(IAgent agent);
}