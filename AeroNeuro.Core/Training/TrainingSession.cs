using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// Orchestrates the training process using an evolution trainer.
/// </summary>
public class TrainingSession<T>
{
    private readonly IEvolutionTrainer<T> _trainer;
    private readonly IEnvironment<T>? _environment;
    private readonly IEnumerable<ITrainingSessionHook<T>> _hooks;

    public TrainingSession(IEvolutionTrainer<T> trainer, IEnvironment<T>? environment,
        IEnumerable<ITrainingSessionHook<T>> hooks)
    {
        _trainer = trainer;
        _environment = environment;
        _hooks = hooks ?? new List<ITrainingSessionHook<T>>();
    }

    /// <summary>
    /// Runs the training for a specified number of generations.
    /// </summary>
    /// <param name="generations">Number of generations to evolve.</param>
    public void Run(int generations)
    {
        var context = new TrainingContext<T>(_trainer, _environment);

        foreach (var hook in _hooks) hook.OnSessionStart(context);

        for (int i = 0; i < generations; i++)
        {
            if (context.ShouldStop) break;

            context.Stats = _trainer.GetStats();
            foreach (var hook in _hooks) hook.OnGenerationStart(context);

            _trainer.EvolveGeneration();

            context.Stats = _trainer.GetStats();
            foreach (var hook in _hooks) hook.OnGenerationEnd(context);
        }

        foreach (var hook in _hooks) hook.OnSessionEnd(context);
    }

    /// <summary>
    /// Runs the training until a stopping condition is met.
    /// </summary>
    /// <param name="stopCondition">Function that returns true when training should stop.</param>
    public void RunUntil(Func<EvolutionStats, bool> stopCondition)
    {
        var context = new TrainingContext<T>(_trainer, _environment);

        foreach (var hook in _hooks) hook.OnSessionStart(context);

        while (!context.ShouldStop)
        {
            context.Stats = _trainer.GetStats();
            foreach (var hook in _hooks) hook.OnGenerationStart(context);

            _trainer.EvolveGeneration();
            EvolutionStats stats = _trainer.GetStats();
            context.Stats = stats;

            foreach (var hook in _hooks) hook.OnGenerationEnd(context);

            if (stopCondition(stats))
            {
                break;
            }
        }

        foreach (var hook in _hooks) hook.OnSessionEnd(context);
    }
}