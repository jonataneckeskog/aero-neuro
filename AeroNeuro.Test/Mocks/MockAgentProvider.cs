using Moq;
using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Test.Mocks;

public static class MockAgentProvider
{
    public static Mock<IAgentProvider<float>> Create(List<IAgent<float>>? agents = null)
    {
        Mock<IAgentProvider<float>> mock = new Mock<IAgentProvider<float>>();

        if (agents is not null && agents.Any())
        {
            Queue<IAgent<float>> agentQueue = new Queue<IAgent<float>>(agents);
            mock.Setup(p => p.CreateRandomAgent(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(agentQueue.Dequeue);
        }
        else
        {
            mock.Setup(p => p.CreateRandomAgent(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => MockAgent.Create().Object);
        }

        return mock;
    }
}
