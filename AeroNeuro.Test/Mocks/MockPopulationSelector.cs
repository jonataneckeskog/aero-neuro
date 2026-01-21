using Moq;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Training;

namespace AeroNeuro.Test.Mocks;

public static class MockPopulationSelector
{
    public static Mock<IPopulationSelector> Create(Func<List<(float Fitness, IAgent Agent)>, List<IAgent>>? selectPopulationFunc = null)
    {
        Mock<IPopulationSelector> mock = new Mock<IPopulationSelector>();

        // If a specific selection function is provided, set up the mock to use it.
        if (selectPopulationFunc != null)
        {
            mock.Setup(x => x.SelectPopulation(It.IsAny<List<(float, IAgent)>>()))
                .Returns(selectPopulationFunc);
        }
        else
        {
            // Otherwise, setup a default behavior to return the agents from the evaluated population.
            mock.Setup(x => x.SelectPopulation(It.IsAny<List<(float, IAgent)>>()))
                .Returns<List<(float Fitness, IAgent Agent)>>(evaluatedPopulation =>
                    evaluatedPopulation.Select(tuple => tuple.Agent).ToList());
        }

        return mock;
    }
}
