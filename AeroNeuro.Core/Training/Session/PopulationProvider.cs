using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

public class PopulationProvider<T> : IPopulationProvider<T>
{
    /// <inheritdoc/>
    public HashSet<IAgent<T>> Population { get; set; } = new();

    /// <inheritdoc/>
    public List<(float Fitness, IAgent<T> Agent)> RankedPopulation { get; set; } = new();

    /// <inheritdoc/>
    public int PopulationSize { get; set; } = new();

    /// <inheritdoc/>
    public IEnumerable<(float Fitness, IInspectableAgent Agent)> GetInspectablePopulation() =>
        RankedPopulation.Select(x => (x.Item1, (IInspectableAgent)x.Item2));
}