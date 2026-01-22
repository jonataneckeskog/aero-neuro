using Moq;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Training;

namespace AeroNeuro.Test.Mocks;

public static class MockFitnessEvaluator
{
    public static Mock<IFitnessEvaluator<float>> Create(Func<IAgent<float>, float>? evaluateFunc = null)
    {
        Mock<IFitnessEvaluator<float>> mock = new Mock<IFitnessEvaluator<float>>();

        // If a specific evaluation function is provided, set up the mock to use it.
        if (evaluateFunc != null)
        {
            mock.Setup(x => x.Evaluate(It.IsAny<IAgent<float>>()))
                .Returns<IAgent<float>>(evaluateFunc);
        }
        else
        {
            // Otherwise, setup a default behavior to return 0.
            mock.Setup(x => x.Evaluate(It.IsAny<IAgent<float>>()))
                .Returns(0f);
        }

        return mock;
    }
}
