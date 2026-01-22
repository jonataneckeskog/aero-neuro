using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Orchestrates the training process using an evolution trainer.
/// </summary>
public class TrainingSession<T>
{
    private readonly IEvolutionTrainer<T> _trainer;
    private readonly IStatsDisplayer _statsDisplayer;
    private readonly IAgentPersistence<T>? _agentPersistence;
    private readonly IEnvironmentDisplayer<T>? _environmentDisplayer;
    private readonly IEnvironment<T>? _environment;

    public TrainingSession(IEvolutionTrainer<T> trainer, IStatsDisplayer statsDisplayer,
        IAgentPersistence<T>? agentPersistence, IEnvironmentDisplayer<T>? environmentDisplayer,
        IEnvironment<T>? environment)
    {
        _trainer = trainer;
        _statsDisplayer = statsDisplayer;
        _agentPersistence = agentPersistence;
        _environmentDisplayer = environmentDisplayer;
        _environment = environment;
    }

    /// <summary>
    /// Runs the training for a specified number of generations.
    /// </summary>
    /// <param name="generations">Number of generations to evolve.</param>
    public void Run(int generations)
    {
        for (int i = 0; i < generations; i++)
        {
            _trainer.EvolveGeneration();
            _statsDisplayer.DisplayStats(_trainer.GetStats());
        }
    }

    /// <summary>
    /// Runs the training until a stopping condition is met.
    /// </summary>
    /// <param name="stopCondition">Function that returns true when training should stop.</param>
    public void RunUntil(Func<EvolutionStats, bool> stopCondition)
    {
        while (true)
        {
            _trainer.EvolveGeneration();
            EvolutionStats stats = _trainer.GetStats();
            _statsDisplayer.DisplayStats(stats);

            if (stopCondition(stats))
            {
                break;
            }
        }
    }
}