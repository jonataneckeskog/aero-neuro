using AeroNeuro.Core.Agents;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Interface for selecting a population of agents based on their fitness.
/// </summary>
public interface IPopulationSelector<T>
{
    /// <summary>
    /// Selects a few agents based on their fitness.
    /// </summary>
    /// <param name="evaluatedPopulation">A list of tuples containing fitness and corresponding agents.</param>
    /// <returns>A list of selected agents for the next generation.</returns>
    List<IAgent<T>> SelectPopulation(List<(float Fitness, IAgent<T> Agent)> evaluatedPopulation);
}