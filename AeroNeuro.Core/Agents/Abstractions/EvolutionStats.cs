namespace AeroNeuro.Core.Agents.Abstractions;

/// <summary>
/// Statistics about the current state of evolution training.
/// </summary>
public record EvolutionStats(
    int Generation,
    float BestFitness,
    float AverageFitness,
    float WorstFitness,
    TimeSpan GenerationDuration
);