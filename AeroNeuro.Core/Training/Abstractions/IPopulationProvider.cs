using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Abstractions;

/// <summary>
/// Interface to handle a population of agents.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPopulationProvider<T>
{
    /// <summary>
    /// The population of IAgents.
    /// </summary>
    HashSet<IAgent<T>> Population { get; set; }

    /// <summary>
    /// Utility to get the population in a read-only format for UI or logging.
    /// </summary>
    IEnumerable<IInspectableAgent> GetInspectablePopulation();

    /// <summary>
    /// The size of the current population.
    /// </summary>
    int PopulationSize { get; set; }
}
