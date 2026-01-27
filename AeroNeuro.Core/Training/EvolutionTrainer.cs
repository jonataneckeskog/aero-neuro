using System.Diagnostics;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;
using AeroNeuro.Core.Exceptions;
using AeroNeuro.Core.Training.Evaluation;
using AeroNeuro.Core.Training.Selection;
using AeroNeuro.Core.Common;

namespace AeroNeuro.Core.Training;

public class EvolutionTrainer<T> : IEvolutionTrainer<T>
{
    private readonly IPopulationProvider<T> _populationProvider;
    private readonly IEvolutionStatsProvider _trainingStatsProvider;
    private readonly IFitnessEvaluator<T> _fitnessEvaluator;
    private readonly IPopulationSelector<T> _populationSelector;
    private readonly int _populationSize;

    public EvolutionTrainer(
        IPopulationProvider<T> populationProvider,
        IEvolutionStatsProvider trainingStatsProvider,
        IFitnessEvaluator<T> fitnessEvaluator,
        IPopulationSelector<T> populationSelector,
        int populationSize)
    {
        _populationProvider = populationProvider;
        _trainingStatsProvider = trainingStatsProvider;
        _fitnessEvaluator = fitnessEvaluator;
        _populationSelector = populationSelector;
        _populationSize = populationSize;
    }

    /// <inheritdoc/>
    public void EvolveGeneration()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Evaluate and sort population by fitness
        List<(float Fitness, IAgent<T> Agent)> rankedPopulation = new List<(float Fitness, IAgent<T> Agent)>();

        foreach (IAgent<T> agent in _populationProvider.Population)
        {
            float fitness = _fitnessEvaluator.Evaluate(agent);
            rankedPopulation.Add((fitness, agent));
        }

        _populationProvider.RankedPopulation = rankedPopulation.OrderByDescending(x => x.Fitness).ToList();

        float bestFitness = _populationProvider.RankedPopulation.First().Fitness;
        float worstFitness = _populationProvider.RankedPopulation.Last().Fitness;
        double averageFitness = _populationProvider.RankedPopulation.Average(x => x.Fitness);

        // Create next generation
        List<IAgent<T>> elites = _populationSelector.SelectPopulation(_populationProvider.RankedPopulation);
        HashSet<IAgent<T>> nextGeneration = new HashSet<IAgent<T>>();

        if (elites.Count == 0)
        {
            throw EvolutionException.EmptySelection();
        }

        nextGeneration.UnionWith(elites);

        // Fill the rest of the population with mutants
        while (nextGeneration.Count < _populationSize)
        {
            IAgent<T> parent = elites[nextGeneration.Count % elites.Count];
            IAgent<T> child = parent.Clone();
            child.Mutate();

            nextGeneration.Add(child);
        }

        stopwatch.Stop();

        _populationProvider.Population = nextGeneration;

        int newGeneration = _trainingStatsProvider.GetCurrentStats().Generation + 1;
        _trainingStatsProvider.Update(new EvolutionStats(
            newGeneration,
            bestFitness,
            (float)averageFitness,
            worstFitness,
            stopwatch.Elapsed
        ));
    }

    /// <inheritdoc/>
    public List<IAgent<T>> GetBestAgents(int count)
    {
        return _populationProvider.RankedPopulation
            .Take(count)
            .Select(x => x.Agent)
            .ToList();
    }

    /// <inheritdoc/>
    public EvolutionStats GetStats()
    {
        return _trainingStatsProvider.GetCurrentStats();
    }
}
