using Moq;
using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Test.Mocks;

public static class MockAgent
{
    public static Mock<IAgent<float>> Create()
    {
        Mock<IAgent<float>> mock = new Mock<IAgent<float>>();

        mock.Setup(agent => agent.Decide(It.IsAny<float[]>()))
            .Returns([]);

        mock.Setup(agent => agent.Clone()).Returns(() => Create().Object);

        return mock;
    }

    public static List<IAgent<float>> CreateList(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Create().Object)
            .ToList();
    }
}