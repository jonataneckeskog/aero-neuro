using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Selection;
using AeroNeuro.Test.Mocks;

namespace AeroNeuro.Test.Training;

public class TopFractionPopulationSelectorTests
{
    private static List<(float Fitness, IAgent<float> Agent)> CreateEvaluatedPopulation(int count)
    {
        return MockAgent.CreateList(count)
            .Select((agent, index) => ((float)count - index, agent))
            .ToList();
    }

    [Fact]
    public void SelectPopulation_SelectsTopHalf()
    {
        OutlierPopulationSelector<float> selector = new TopFractionPopulationSelector<float>(0.5f);
        List<(float Fitness, IAgent<float> Agent)> evaluatedPopulation = CreateEvaluatedPopulation(10);
        List<IAgent<float>> selectedAgents = selector.SelectPopulation(evaluatedPopulation);
        Assert.Equal(5, selectedAgents.Count);
        Assert.Equal(evaluatedPopulation[0].Agent, selectedAgents[0]);
        Assert.Equal(evaluatedPopulation[4].Agent, selectedAgents[4]);
    }

    [Fact]
    public void SelectPopulation_FractionResultsInZero_SelectsOne()
    {
        OutlierPopulationSelector<float> selector = new TopFractionPopulationSelector<float>(0.01f);
        List<(float Fitness, IAgent<float> Agent)> evaluatedPopulation = CreateEvaluatedPopulation(10);
        List<IAgent<float>> selectedAgents = selector.SelectPopulation(evaluatedPopulation);
        Assert.Single(selectedAgents);
        Assert.Equal(evaluatedPopulation[0].Agent, selectedAgents[0]);
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(1.1f)]
    public void Constructor_InvalidFraction_ThrowsException(float fraction)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TopFractionPopulationSelector<float>(fraction));
    }

    [Fact]
    public void SelectPopulation_EmptyPopulation_ReturnsEmptyList()
    {
        OutlierPopulationSelector<float> selector = new TopFractionPopulationSelector<float>(0.5f);
        List<(float Fitness, IAgent<float> Agent)> evaluatedPopulation = new List<(float, IAgent<float>)>();
        List<IAgent<float>> selectedAgents = selector.SelectPopulation(evaluatedPopulation);
        Assert.Empty(selectedAgents);
    }
}
