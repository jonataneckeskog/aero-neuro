using Moq;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Test.Mocks;

// Reusable Mock Environment
public class MockEnvironment : Mock<IEnvironment>
{
    private int _stepCount;

    public MockEnvironment(float rewardToReturn, int stepsUntilDone = int.MaxValue)
    {
        // 1. Setup Default Observation (to avoid null references)
        Setup(x => x.GetObservation()).Returns([]);

        // 2. Setup Step Logic: Return reward + Increment internal counter
        Setup(x => x.Step(It.IsAny<float[]>()))
            .Returns(rewardToReturn)
            .Callback(() => _stepCount++);

        // 3. Setup IsDone Logic: Check internal counter against limit
        Setup(x => x.IsDone)
            .Returns(() => _stepCount >= stepsUntilDone);
    }

    // Helper property to check steps in tests easily without using Verify() syntax if preferred
    public int StepCount => _stepCount;
}
