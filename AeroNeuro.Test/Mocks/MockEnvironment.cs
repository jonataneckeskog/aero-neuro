using Moq;
using AeroNeuro.Core.Environments.Abstractions;

namespace AeroNeuro.Test.Mocks;

public static class MockEnvironment
{
    public static Mock<IEnvironment<float>> Create(float rewardToReturn, int stepsUntilDone = int.MaxValue)
    {
        Mock<IEnvironment<float>> mock = new Mock<IEnvironment<float>>();
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
