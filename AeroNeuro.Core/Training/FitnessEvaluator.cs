using System.Diagnostics;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Evaluates the fitness of an agent using given rewards from the environment.
/// </summary>
public class TrainingFitnessEvaluator<T> : IFitnessEvaluator<T>
{
    private readonly IEnvironment<T> _environment;
    private readonly int _episodes;
    private readonly int _maxSteps;
    private readonly int _timeWeight;

    public TrainingFitnessEvaluator(IEnvironment<T> environment, int episodes, int timeWeight, int maxSteps)
    {
        _environment = environment;
        _episodes = episodes;
        _timeWeight = timeWeight;
        _maxSteps = maxSteps;
    }

    /// <inheritdoc/>
    public float Evaluate(IAgent<T> agent)
    {
        float totalFitness = 0;
        long startTimestamp = Stopwatch.GetTimestamp();

        for (int e = 0; e < _episodes; e++)
        {
            _environment.Reset();
            float episodeReward = 0;
            int i = 0;
            while (!_environment.IsDone && i < _maxSteps)
            {
                T[] observation = _environment.GetObservation();
                T[] actions = agent.Decide(observation);
                episodeReward += _environment.Step(actions);
                i++;
            }
            totalFitness += episodeReward * (e + 1);
        }

        long endTimestamp = Stopwatch.GetTimestamp();
        double elapsedSeconds = Stopwatch.GetElapsedTime(startTimestamp, endTimestamp).TotalSeconds;
        float timePenalty = (float)(elapsedSeconds * _timeWeight);

        return totalFitness - timePenalty;
    }
}