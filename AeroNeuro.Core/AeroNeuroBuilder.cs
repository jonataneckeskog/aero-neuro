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
    private IAgentPersistence<T>? _agentPersistence;
    private IStatsDisplayer? _statsDisplayer;
    private IEnvironmentDisplayer<T>? _environmentDisplayer;
    private int _populationSize = 100;
    private float _maxStepsPerEpisode = 1000f;

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
    /// Sets the population selector. Defaults to TopFractionPopulationSelector(0.2).
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
    /// Sets the agent persistence.
    /// </summary>
    public AeroNeuroBuilder<T> WithAgentPersistence(IAgentPersistence<T> agentPersistence)
    {
        _agentPersistence = agentPersistence;
        return this;
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
    /// Sets the environment displayer.
    /// </summary>
    public AeroNeuroBuilder<T> WithEnvironmentDisplayer(IEnvironmentDisplayer<T> environmentDisplayer)
    {
        _environmentDisplayer = environmentDisplayer;
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
    public AeroNeuroBuilder<T> WithMaxStepsPerEpisode(float steps)
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
            || _agentPersistence is null || _statsDisplayer is null)
        {
            throw new InvalidOperationException("Missing mandatory components.");
        }

        IEvolutionTrainer<T> trainer = new EvolutionTrainer<T>(
            _agentProvider,
            _fitnessEvaluator,
            _populationSelector,
            _populationSize
        );

        return new TrainingSession<T>(trainer, _statsDisplayer, _agentPersistence, _environmentDisplayer, _environment);
    }
}