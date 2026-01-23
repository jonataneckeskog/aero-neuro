using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Training;

/// <summary>
/// Orchestrates the training process using an evolution trainer.
/// </summary>
public class TrainingSession<T>
{
    private readonly IEvolutionTrainer<T> _trainer;
    private readonly IStatsDisplayer _statsDisplayer;
    private readonly IEnvironment<T>? _environment;
    private readonly IEnumerable<ITrainingSessionHook<T>> _hooks;

    public TrainingSession(IEvolutionTrainer<T> trainer, IStatsDisplayer statsDisplayer,
        IEnvironment<T>? environment, IEnumerable<ITrainingSessionHook<T>> hooks)
    {
        _trainer = trainer;
        _statsDisplayer = statsDisplayer;
        _environment = environment;
        _hooks = hooks ?? new List<ITrainingSessionHook<T>>();
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

            foreach (var hook in _hooks)
            {
                hook.OnGenerationEvolved(_trainer.GetStats(), _trainer, _environment);
            }
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

            foreach (var hook in _hooks)
            {
                hook.OnGenerationEvolved(stats, _trainer, _environment);
            }

            if (stopCondition(stats))
            {
                break;
            }
        }
    }
}