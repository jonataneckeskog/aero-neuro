using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Selection;

public class TopFractionPopulationSelector<T> : IPopulationSelector<T>
{
    private readonly float _eliteFraction;

    public TopFractionPopulationSelector(float eliteFraction)
    {
        if (eliteFraction < 0f || eliteFraction > 1f)
        {
            throw new ArgumentOutOfRangeException(nameof(eliteFraction), "Elite fraction must be between 0 and 1.");
        }

        _eliteFraction = eliteFraction;
    }

    /// <inheritdoc/>
    public List<IAgent<T>> SelectPopulation(List<(float Fitness, IAgent<T> Agent)> evaluatedPopulation)
    {
        int eliteCount = (int)(evaluatedPopulation.Count * _eliteFraction);

        if (eliteCount == 0 && evaluatedPopulation.Count > 0)
        {
            eliteCount = 1;
        }

        return evaluatedPopulation
            .Take(eliteCount)
            .Select(x => x.Agent)
            .ToList();
    }
}