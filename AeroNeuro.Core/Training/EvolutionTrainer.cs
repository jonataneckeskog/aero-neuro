using System.Diagnostics;
using AeroNeuro.Core.Agents;

namespace AeroNeuro.Core.Training;

public class EvolutionTrainer : IEvolutionTrainer
{
    private readonly IAgentProvider _agentProvider;
    private readonly IFitnessEvaluator _fitnessEvaluator;
    private readonly IPopulationSelector _populationSelector;
    private readonly int _populationSize;

    private List<(float Fitness, IAgent Agent)> _bestPopulation;
    private List<IAgent> _population;
    private EvolutionStats _currentStats;

    public EvolutionTrainer(IAgentProvider agentProvider, IFitnessEvaluator fitnessEvaluator, IPopulationSelector populationSelector, int populationSize = 100)
    {
        _agentProvider = agentProvider;
        _fitnessEvaluator = fitnessEvaluator;
        _populationSelector = populationSelector;
        _populationSize = populationSize;

        _bestPopulation = new List<(float, IAgent)>();
        _population = new List<IAgent>();

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
        List<(float Fitness, IAgent Agent)> rankedPopulation = new List<(float Fitness, IAgent Agent)>();

        foreach (IAgent agent in _population)
        {
            float fitness = _fitnessEvaluator.Evaluate(agent);
            rankedPopulation.Add((fitness, agent));
        }

        _bestPopulation = rankedPopulation.OrderByDescending(x => x.Fitness).ToList();

        float bestFitness = _bestPopulation.First().Fitness;
        float worstFitness = _bestPopulation.Last().Fitness;
        double averageFitness = _bestPopulation.Average(x => x.Fitness);

        // Create next generation
        List<IAgent> elites = _populationSelector.SelectPopulation(_bestPopulation);
        List<IAgent> nextGeneration = new List<IAgent>();

        // Add elites to next generation
        foreach (IAgent elite in elites)
        {
            nextGeneration.Add(elite.Clone());
        }

        // Fill the rest of the population with mutants
        while (nextGeneration.Count < _populationSize)
        {
            IAgent parent = elites[nextGeneration.Count % elites.Count];
            IAgent child = parent.Clone();
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
    public List<IAgent> GetBestAgents(int count)
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