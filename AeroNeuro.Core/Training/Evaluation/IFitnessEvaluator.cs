using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Evaluation;

/// <summary>
/// Interface for evaluating the fitness of agents. Higher fitness indicates better performance.
/// </summary>
public interface IFitnessEvaluator<T>
{
    /// <summary>
    /// Evaluates a population of agents. 
    /// This allows for batch processing or competitive evaluation (e.g. Tournaments).
    /// </summary>
    void EvaluatePopulation(IPopulationProvider<T> populationProvider);
}
