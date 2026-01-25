using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// A generic hook that executes an action if a predicate is met.
/// </summary>
public class DelegateTrainingHook<T> : ITrainingSessionHook<T>
{
    private readonly Func<EvolutionStats, bool> _predicate;
    private readonly Action<EvolutionStats, IEvolutionTrainer<T>, IEnvironment<T>?> _action;

    public DelegateTrainingHook(Func<EvolutionStats, bool> predicate, Action<EvolutionStats, IEvolutionTrainer<T>, IEnvironment<T>?> action)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <inheritdoc/>
    public void OnGenerationEnd(TrainingContext<T> context)
    {
        if (context.Stats != null && _predicate(context.Stats))
        {
            _action(context.Stats, context.Trainer, context.Environment);
        }
    }
}
