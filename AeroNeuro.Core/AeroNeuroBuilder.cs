using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training;
using AeroNeuro.Core.Training.Abstractions;
using AeroNeuro.Core.Training.Evaluation;
using AeroNeuro.Core.Training.Selection;
using AeroNeuro.Core.Training.Session;

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
    private int _populationSize = 0;
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
    /// Sets the population selector.
    /// </summary>
    public AeroNeuroBuilder<T> WithPopulationSelector(IPopulationSelector<T> selector)
    {
        _populationSelector = selector;
        return this;
    }

    /// <summary>
    /// Sets a custom fitness evaluator.
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
    /// Sets the population size.
    /// </summary>
    public AeroNeuroBuilder<T> WithPopulationSize(int size)
    {
        _populationSize = size;
        return this;
    }

    /// <summary>
    /// Builds the training session with the configured components. Crashes if mandatory
    /// components are not set.
    /// </summary>
    public TrainingSession<T> Build()
    {
        if (_environment is null || _agentProvider is null
            || _populationSelector is null || _fitnessEvaluator is null || _populationSize == 0)
        {
            throw new InvalidOperationException("Missing mandatory components.");
        }

        IEvolutionTrainer<T> trainer = new EvolutionTrainer<T>(
            _agentProvider,
            _fitnessEvaluator,
            _populationSelector,
            _populationSize
        );

        return new TrainingSession<T>(trainer, _environment, _hooks);
    }
}