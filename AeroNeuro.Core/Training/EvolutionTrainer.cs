using System.Diagnostics;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Exceptions;

namespace AeroNeuro.Core.Training;

public class EvolutionTrainer<T> : IEvolutionTrainer<T>
{
    private readonly IAgentProvider<T> _agentProvider;
    private readonly IFitnessEvaluator<T> _fitnessEvaluator;
    private readonly IPopulationSelector<T> _populationSelector;
    private readonly int _populationSize;

    private List<(float Fitness, IAgent<T> Agent)> _bestPopulation;
    private List<IAgent<T>> _population;
    private EvolutionStats _currentStats;

    public EvolutionTrainer(IAgentProvider<T> agentProvider, IFitnessEvaluator<T> fitnessEvaluator, IPopulationSelector<T> populationSelector, int populationSize = 100)
    {
        _agentProvider = agentProvider;
        _fitnessEvaluator = fitnessEvaluator;
        _populationSelector = populationSelector;
        _populationSize = populationSize;

        _bestPopulation = new List<(float, IAgent<T>)>();
        _population = new List<IAgent<T>>();

        // Initialize stats
        _currentStats = new EvolutionStats(0, 0, 0, 0, TimeSpan.Zero);

        InitializePopulation();
    }

    private void InitializePopulation()
    {
        for (int i = 0; i < _populationSize; i++)
        {
            _population.Add(_agentProvider.CreateRandomAgent(10, 100));
        }

        _bestPopulation = _population.Select(a => (0f, a)).ToList();
    }

    /// <inheritdoc/>
    public void EvolveGeneration()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Evaluate and sort population by fitness
        List<(float Fitness, IAgent<T> Agent)> rankedPopulation = new List<(float Fitness, IAgent<T> Agent)>();

        foreach (IAgent<T> agent in _population)
        {
            float fitness = _fitnessEvaluator.Evaluate(agent);
            rankedPopulation.Add((fitness, agent));
        }

        _bestPopulation = rankedPopulation.OrderByDescending(x => x.Fitness).ToList();

        float bestFitness = _bestPopulation.First().Fitness;
        float worstFitness = _bestPopulation.Last().Fitness;
        double averageFitness = _bestPopulation.Average(x => x.Fitness);

        // Create next generation
        List<IAgent<T>> elites = _populationSelector.SelectPopulation(_bestPopulation);
        List<IAgent<T>> nextGeneration = new List<IAgent<T>>();

        if (elites.Count == 0)
        {
            throw EvolutionException.EmptySelection();
        }

        // Add elites to next generation
        nextGeneration.AddRange(elites);

        // Fill the rest of the population with mutants
        while (nextGeneration.Count < _populationSize)
        {
            IAgent<T> parent = elites[nextGeneration.Count % elites.Count];
            IAgent<T> child = parent.Clone();
            child.Mutate();

            nextGeneration.Add(child);
        }

        _population = nextGeneration;
        stopwatch.Stop();

        int newGeneration = _currentStats.Generation + 1;

        _currentStats = new EvolutionStats(
            newGeneration,
            bestFitness,
            (float)averageFitness,
            worstFitness,
            stopwatch.Elapsed);
    }

    /// <inheritdoc/>
    public List<IAgent<T>> GetBestAgents(int count)
    {
        return _bestPopulation
            .Take(count)
            .Select(x => x.Agent)
            .ToList();
    }

    /// <inheritdoc/>
    public EvolutionStats GetStats()
    {
        return _currentStats;
    }
}