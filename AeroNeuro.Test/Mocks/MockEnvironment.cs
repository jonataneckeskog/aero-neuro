using Moq;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Test.Mocks;

public static class MockEnvironment
{
    public static Mock<IEnvironment> Create(float rewardToReturn, int stepsUntilDone = int.MaxValue)
    {
        Mock<IEnvironment> mock = new Mock<IEnvironment>();
        int stepCount = 0;

        mock.Setup(env => env.GetObservation()).Returns([]);
        
        mock.Setup(env => env.Step(It.IsAny<float[]>()))
            .Returns(rewardToReturn)
            .Callback(() => stepCount++);
        
        mock.Setup(env => env.IsDone)
            .Returns(() => stepCount >= stepsUntilDone);
        
        mock.Setup(env => env.Reset())
            .Callback(() => stepCount = 0);

        return mock;
    }
}
