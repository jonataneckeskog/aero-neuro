using Moq;
using AeroNeuro.Core.Agents;

namespace AeroNeuro.Test.Mocks;

public static class MockAgent
{
    public static Mock<IAgent> Create()
    {
        Mock<IAgent> mock = new Mock<IAgent>();
        // Setup default decision to avoid nulls
        mock.Setup(x => x.Decide(It.IsAny<float[]>()))
            .Returns([]);
        return mock;
    }
}