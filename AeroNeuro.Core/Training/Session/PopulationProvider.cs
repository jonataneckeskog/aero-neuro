using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

public class PopulationProvider<T> : IPopulationProvider<T>
{
    /// <inhetdoc/>
    public HashSet<IAgent<T>> Population { get; set; } = new();

    /// <inhetdoc/>
    public int PopulationSize { get; set; } = new();

    /// <inhetdoc/>
    public IEnumerable<IInspectableAgent> GetInspectablePopulation() => Population;
}