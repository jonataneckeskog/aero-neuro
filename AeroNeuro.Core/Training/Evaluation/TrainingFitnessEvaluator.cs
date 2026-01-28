using System.Diagnostics;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Evaluation;

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
    public void EvaluatePopulation(IPopulationProvider<T> populationProvider)
    {
        var agents = populationProvider.Population;
        if (agents == null || agents.Count == 0) return;

        // Temporary storage for this evaluation run
        var fitnessScores = agents.ToDictionary(a => a, _ => 0f);

        long startTimestamp = Stopwatch.GetTimestamp();

        for (int e = 0; e < _episodes; e++)
        {
            _environment.Reset();
            int step = 0;

            while (!_environment.IsDone && step < _maxSteps)
            {
                T[] observation = _environment.GetObservation();

                // Map to store actions for the current frame so we can Act first, then Step later
                var currentFrameActions = new Dictionary<IAgent<T>, T[]>(agents.Count);

                // --- PHASE 1: DECIDE & ACT ---
                // All agents declare their intentions first
                foreach (var agent in agents)
                {
                    T[] action = agent.Decide(observation);
                    currentFrameActions[agent] = action;
                    _environment.Act(action);
                }

                // --- PHASE 2: STEP & EVALUATE ---
                // Now resolve the consequences for each agent
                foreach (var kvp in currentFrameActions)
                {
                    var agent = kvp.Key;
                    var action = kvp.Value;

                    float reward = _environment.Step(action);
                    fitnessScores[agent] += reward * (e + 1);
                }

                step++;
            }
        }

        // --- PHASE 3: FINALIZE & RANK ---
        long endTimestamp = Stopwatch.GetTimestamp();
        double elapsedSeconds = Stopwatch.GetElapsedTime(startTimestamp, endTimestamp).TotalSeconds;
        float timePenalty = (float)(elapsedSeconds * _timeWeight);

        // Create the new ranked list by mapping agents to their calculated scores
        populationProvider.RankedPopulation = agents
            .Select(agent =>
            {
                float finalFitness = fitnessScores[agent] - timePenalty;
                return (Fitness: finalFitness, Agent: agent);
            })
            .OrderByDescending(x => x.Fitness)
            .ToList();
    }
}