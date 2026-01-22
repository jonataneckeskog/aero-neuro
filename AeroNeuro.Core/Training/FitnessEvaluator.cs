using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Evaluates the fitness of an agent using given rewards from the environment.
/// </summary>
public class TrainingFitnessEvaluator<T> : IFitnessEvaluator<T>
{
    private readonly IEnvironment<T> _environment;
    private readonly float _repetitions;

    public TrainingFitnessEvaluator(IEnvironment<T> environment, float repetitions)
    {
        _environment = environment;
        _repetitions = repetitions;
    }

    /// <inheritdoc/>
    public float Evaluate(IAgent<T> agent)
    {
        _environment.Reset();
        float totalReward = 0;
        int i = 0;
        while (!_environment.IsDone && i < _repetitions)
        {
            T[] observation = _environment.GetObservation();
            T[] actions = agent.Decide(observation);
            totalReward += _environment.Step(actions);
            i++;
        }
        return totalReward;
    }
}