namespace AeroNeuro.Core.Training;

public class StatsDisplayer : IStatsDisplayer
{
    /// <inheritdoc/>
    public void DisplayStats(EvolutionStats stats)
    {
        Console.WriteLine($"Generation: {stats.Generation}, Best Fitness: {stats.BestFitness}, Average Fitness: {stats.AverageFitness}");
    }
}