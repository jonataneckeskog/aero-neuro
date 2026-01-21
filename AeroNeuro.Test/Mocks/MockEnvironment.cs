using Moq;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Test.Mocks;

public static class MockEnvironment
{
    public static Mock<IEnvironment> Create(float rewardToReturn, int stepsUntilDone = int.MaxValue)
    {
        Mock<IEnvironment> mock = new Mock<IEnvironment>();
        int stepCount = 0;

        // 1. Setup Default Observation (to avoid null references)
        mock.Setup(x => x.GetObservation()).Returns([]);

        // 2. Setup Step Logic: Return reward + Increment internal counter
        mock.Setup(x => x.Step(It.IsAny<float[]>()))
            .Returns(rewardToReturn)
            .Callback(() => stepCount++);

        // 3. Setup IsDone Logic: Check internal counter against limit
        mock.Setup(x => x.IsDone)
            .Returns(() => stepCount >= stepsUntilDone);
        
        mock.Setup(x => x.Reset())
            .Callback(() => stepCount = 0);

        return mock;
    }
}
