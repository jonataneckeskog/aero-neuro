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

        _fitnessEvaluator.EvaluatePopulation(_populationProvider);
        var rankedPop = _populationProvider.RankedPopulation;

        if (rankedPop == null || rankedPop.Count == 0)
        {
            throw new InvalidOperationException("Evaluation failed to produce a ranked population.");
        }

        float bestFitness = rankedPop[0].Fitness;
        float worstFitness = rankedPop[rankedPop.Count - 1].Fitness;
        double averageFitness = rankedPop.Average(x => x.Fitness);

        List<IAgent<T>> elites = _populationSelector.SelectPopulation(rankedPop);
        HashSet<IAgent<T>> nextGeneration = new HashSet<IAgent<T>>();

        if (elites.Count == 0)
        {
            throw EvolutionException.EmptySelection();
        }

        nextGeneration.UnionWith(elites);

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
