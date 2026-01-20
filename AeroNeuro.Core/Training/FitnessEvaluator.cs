using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Evaluates the fitness of an agent using given rewards from the environment.
/// </summary>
public class TrainingFitnessEvaluator : IFitnessEvaluator
{
    private readonly IEnvironment _environment;
    private readonly float _repetitions;

    public TrainingFitnessEvaluator(IEnvironment environment, float repetitions)
    {
        _environment = environment;
        _repetitions = repetitions;
    }

    /// <inheritdoc/>
    public float Evaluate(IAgent agent)
    {
        _environment.Reset();
        float totalReward = 0;
        int i = 0;
        while (!_environment.IsDone && i < _repetitions)
        {
            float[] observation = _environment.GetObservation();
            float[] actions = agent.Decide(observation);
            totalReward += _environment.Step(actions);
            i++;
        }
        return totalReward;
    }
}