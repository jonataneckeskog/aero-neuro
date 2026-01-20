using Moq;
using AeroNeuro.Core.Training;
using AeroNeuro.Test.Mocks;

namespace AeroNeuro.Test;

public class FitnessEvaluatorTests
{
    [Fact]
    public void Evaluate_ShouldReturnCorrectTotalReward()
    {
        // Arrange
        // We instantiate our custom definition which already contains the complex Setup logic
        MockEnvironment mockEnv = new MockEnvironment(rewardToReturn: 10);
        MockAgent mockAgent = new MockAgent();

        TrainingFitnessEvaluator evaluator = new TrainingFitnessEvaluator(mockEnv.Object, 10);

        // Act
        float totalReward = evaluator.Evaluate(mockAgent.Object);

        // Assert
        Assert.Equal(100.0f, totalReward);

        // We can verify using standard Moq syntax
        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(10));

        // OR we can use the helper property we added to our custom class
        Assert.Equal(10, mockEnv.StepCount);
    }

    [Fact]
    public void Evaluate_ShouldStopWhenDone()
    {
        // Arrange
        // Reuse is easy: just pass the specific arguments for this scenario
        MockEnvironment mockEnv = new MockEnvironment(rewardToReturn: 10, stepsUntilDone: 5);
        MockAgent mockAgent = new MockAgent();

        TrainingFitnessEvaluator evaluator = new TrainingFitnessEvaluator(mockEnv.Object, 100);

        // Act
        float totalReward = evaluator.Evaluate(mockAgent.Object);

        // Assert
        Assert.Equal(50.0f, totalReward);
        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(5));
    }
}