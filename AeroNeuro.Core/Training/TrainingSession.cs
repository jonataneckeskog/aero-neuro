using AeroNeuro.Core.Common;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// Orchestrates the training process using an evolution trainer.
/// </summary>
public class TrainingSession<T>
{
    private readonly IEvolutionTrainer<T> _trainer;
    private readonly TrainingContext<T> _context;
    private readonly IEnumerable<ITrainingSessionHook<T>> _hooks;

    public TrainingSession(IEvolutionTrainer<T> trainer,
        TrainingContext<T> context,
        IEnumerable<ITrainingSessionHook<T>> hooks)
    {
        _trainer = trainer;
        _context = context;
        _hooks = hooks ?? new List<ITrainingSessionHook<T>>();
    }

    /// <summary>
    /// Runs the training for a specified number of generations.
    /// </summary>
    /// <param name="generations">Number of generations to evolve.</param>
    public void Run(int generations)
    {
        foreach (var hook in _hooks) hook.OnSessionStart(_context);

        for (int i = 0; i < generations; i++)
        {
            if (_context.ShouldStop) break;

            foreach (var hook in _hooks) hook.OnGenerationStart(_context);

            _trainer.EvolveGeneration();

            foreach (var hook in _hooks) hook.OnGenerationEnd(_context);
        }

        foreach (var hook in _hooks) hook.OnSessionEnd(_context);
    }

    /// <summary>
    /// Runs the training until a stopping condition is met.
    /// </summary>
    /// <param name="stopCondition">Function that returns true when training should stop.</param>
    public void RunUntil(Func<EvolutionStats, bool> stopCondition)
    {
        foreach (var hook in _hooks) hook.OnSessionStart(_context);

        while (!_context.ShouldStop)
        {
            foreach (var hook in _hooks) hook.OnGenerationStart(_context);

            _trainer.EvolveGeneration();
            EvolutionStats stats = _trainer.GetStats();

            foreach (var hook in _hooks) hook.OnGenerationEnd(_context);

            if (stopCondition(stats))
            {
                break;
            }
        }

        foreach (var hook in _hooks) hook.OnSessionEnd(_context);
    }
}