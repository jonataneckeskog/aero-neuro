using Moq;
using AeroNeuro.Core.Agents;

namespace AeroNeuro.Test.Mocks;

public class MockAgent : Mock<IAgent>
{
    public MockAgent()
    {
        // Setup default decision to avoid nulls
        Setup(x => x.Decide(It.IsAny<float[]>()))
            .Returns([]);
    }
}