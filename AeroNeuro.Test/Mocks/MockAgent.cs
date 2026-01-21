using Moq;
using AeroNeuro.Core.Agents;

namespace AeroNeuro.Test.Mocks;

public static class MockAgent
{
    public static Mock<IAgent> Create()
    {
        Mock<IAgent> mock = new Mock<IAgent>();

        mock.Setup(agent => agent.Decide(It.IsAny<float[]>()))
            .Returns([]);

        mock.Setup(agent => agent.Clone()).Returns(() => Create().Object);

        return mock;
    }

    public static List<IAgent> CreateList(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Create().Object)
            .ToList();
    }
}