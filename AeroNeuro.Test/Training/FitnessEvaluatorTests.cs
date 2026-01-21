using Moq;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;
using AeroNeuro.Test.Mocks;

namespace AeroNeuro.Test;

public class FitnessEvaluatorTests
{
    [Fact]
    public void Evaluate_ShouldReturnCorrectTotalReward()
    {
        Mock<IEnvironment> mockEnv = MockEnvironment.Create(rewardToReturn: 10);
        Mock<IAgent> mockAgent = MockAgent.Create();

        TrainingFitnessEvaluator evaluator = new TrainingFitnessEvaluator(mockEnv.Object, 10);

        float totalReward = evaluator.Evaluate(mockAgent.Object);

        Assert.Equal(100.0f, totalReward);

        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(10));
    }

    [Fact]
    public void Evaluate_ShouldStopWhenDone()
    {
        Mock<IEnvironment> mockEnv = MockEnvironment.Create(rewardToReturn: 10, stepsUntilDone: 5);
        Mock<IAgent> mockAgent = MockAgent.Create();

        TrainingFitnessEvaluator evaluator = new TrainingFitnessEvaluator(mockEnv.Object, 100);

        float totalReward = evaluator.Evaluate(mockAgent.Object);

        Assert.Equal(50.0f, totalReward);
        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(5));
    }
}