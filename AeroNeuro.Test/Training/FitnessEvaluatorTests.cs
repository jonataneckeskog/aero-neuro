using Moq;
using AeroNeuro.Test.Mocks;
using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Evaluation;

namespace AeroNeuro.Test.Training;

public class FitnessEvaluatorTests
{
    [Fact]
    public void Evaluate_ShouldReturnCorrectTotalReward()
    {
        Mock<IEnvironment<float>> mockEnv = MockEnvironment.Create(rewardToReturn: 10);
        Mock<IAgent<float>> mockAgent = MockAgent.Create();

        TrainingFitnessEvaluator<float> evaluator = new TrainingFitnessEvaluator<float>(mockEnv.Object, 10);

        float totalReward = evaluator.Evaluate(mockAgent.Object);

        Assert.Equal(100.0f, totalReward);

        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(10));
    }

    [Fact]
    public void Evaluate_ShouldStopWhenDone()
    {
        Mock<IEnvironment<float>> mockEnv = MockEnvironment.Create(rewardToReturn: 10, stepsUntilDone: 5);
        Mock<IAgent<float>> mockAgent = MockAgent.Create();

        TrainingFitnessEvaluator<float> evaluator = new TrainingFitnessEvaluator<float>(mockEnv.Object, 100);

        float totalReward = evaluator.Evaluate(mockAgent.Object);

        Assert.Equal(50.0f, totalReward);
        mockEnv.Verify(env => env.Step(It.IsAny<float[]>()), Times.Exactly(5));
    }
}