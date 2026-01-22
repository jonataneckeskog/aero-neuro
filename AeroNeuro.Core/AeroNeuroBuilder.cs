using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;

namespace AeroNeuro.Core;

/// <summary>
/// Builder for configuring and creating a training session.
/// </summary>
public class AeroNeuroBuilder<T>
{
    private IEnvironment<T>? _environment;
    private IAgentProvider<T>? _agentProvider;
    private IPopulationSelector<T>? _populationSelector;
    private IFitnessEvaluator<T>? _fitnessEvaluator;
    private IStatsDisplayer? _statsDisplayer;
    private int _populationSize = 100;
    private int _maxStepsPerEpisode = 1000;
    private readonly List<ITrainingSessionHook<T>> _hooks = new();

    /// <summary>
    /// Sets the environment for the agents.
    /// </summary>
    public AeroNeuroBuilder<T> WithEnvironment(IEnvironment<T> environment)
    {
        _environment = environment;
        return this;
    }

    /// <summary>
    /// Sets the agent provider for creating agents.
    /// </summary>
    public AeroNeuroBuilder<T> WithAgentProvider(IAgentProvider<T> agentProvider)
    {
        _agentProvider = agentProvider;
        return this;
    }

    /// <summary>
    /// Sets the population selector. Defaults to null.
    /// </summary>
    public AeroNeuroBuilder<T> WithPopulationSelector(IPopulationSelector<T> selector)
    {
        _populationSelector = selector;
        return this;
    }

    /// <summary>
    /// Sets a custom fitness evaluator. If not set, a TrainingFitnessEvaluator will be created using the Environment.
    /// </summary>
    public AeroNeuroBuilder<T> WithFitnessEvaluator(IFitnessEvaluator<T> evaluator)
    {
        _fitnessEvaluator = evaluator;
        return this;
    }

    /// <summary>
    /// Adds a generic training hook.
    /// </summary>
    public AeroNeuroBuilder<T> WithHook(ITrainingSessionHook<T> hook)
    {
        _hooks.Add(hook);
        return this;
    }

    /// <summary>
    /// Adds a conditional action to be executed during training.
    /// </summary>
    public AeroNeuroBuilder<T> WithConditionalAction(Func<EvolutionStats, bool> predicate, Action<EvolutionStats, IEvolutionTrainer<T>, IEnvironment<T>?> action)
    {
        _hooks.Add(new DelegateTrainingHook<T>(predicate, action));
        return this;
    }

    /// <summary>
    /// Adds a conditional action that interacts with the environment.
    /// </summary>
    public AeroNeuroBuilder<T> WithConditionalAction(Func<EvolutionStats, bool> predicate, Action<IEnvironment<T>> action)
    {
        return WithConditionalAction(predicate, (stats, trainer, env) =>
        {
            if (env is not null)
            {
                action(env);
            }
        });
    }

    /// <summary>
    /// Adds a conditional action that interacts with the trainer.
    /// </summary>
    public AeroNeuroBuilder<T> WithConditionalAction(Func<EvolutionStats, bool> predicate, Action<IEvolutionTrainer<T>> action)
    {
        return WithConditionalAction(predicate, (stats, trainer, env) => action(trainer));
    }

    /// <summary>
    /// Sets the stats displayer.
    /// </summary>
    /// <param name="statsDisplayer"></param>
    /// <returns></returns>
    public AeroNeuroBuilder<T> WithStatsDisplayer(IStatsDisplayer statsDisplayer)
    {
        _statsDisplayer = statsDisplayer;
        return this;
    }

    /// <summary>
    /// Sets the population size. Default is 100.
    /// </summary>
    public AeroNeuroBuilder<T> WithPopulationSize(int size)
    {
        _populationSize = size;
        return this;
    }

    /// <summary>
    /// Sets the maximum steps per episode for the default fitness evaluator. Default is 1000.
    /// </summary>
    public AeroNeuroBuilder<T> WithMaxStepsPerEpisode(int steps)
    {
        _maxStepsPerEpisode = steps;
        return this;
    }

    /// <summary>
    /// Builds the training session with the configured components.
    /// </summary>
    public TrainingSession<T> Build()
    {
        if (_environment is null || _agentProvider is null
            || _populationSelector is null || _fitnessEvaluator is null
            || _statsDisplayer is null)
        {
            throw new InvalidOperationException("Missing mandatory components.");
        }

        IEvolutionTrainer<T> trainer = new EvolutionTrainer<T>(
            _agentProvider,
            _fitnessEvaluator,
            _populationSelector,
            _populationSize
        );

        return new TrainingSession<T>(trainer, _statsDisplayer, _environment, _hooks);
    }
}