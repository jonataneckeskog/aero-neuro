using Moq;
using AeroNeuro.Core.Agents;

namespace AeroNeuro.Test.Mocks;

public static class MockAgentProvider
{
    public static Mock<IAgentProvider> Create(List<IAgent>? agents = null)
    {
        Mock<IAgentProvider> mock = new Mock<IAgentProvider>();

        if (agents is not null && agents.Any())
        {
            Queue<IAgent> agentQueue = new Queue<IAgent>(agents);
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
