using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Common;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Decorators;

public class AdversaryEvolutionTrainer<T> : IEvolutionTrainer<T>
{
    private IEvolutionTrainer<T> _trainer { get; }
    private IEvolutionTrainer<T> _adversaryTrainer { get; }

    public AdversaryEvolutionTrainer(IEvolutionTrainer<T> trainer, IEvolutionTrainer<T> adversaryTrainer)
    {
        _trainer = trainer;
        _adversaryTrainer = adversaryTrainer;
    }

    /// <inheritdoc/>
    public void EvolveGeneration()
    {
        _trainer.EvolveGeneration();
        _adversaryTrainer.EvolveGeneration();
    }

    /// <inheritdoc/>
    public List<IAgent<T>> GetBestAgents(int count)
    {
        return _trainer.GetBestAgents(count);
    }

    /// <inheritdoc/>
    public EvolutionStats GetStats()
    {
        return _trainer.GetStats();
    }
}
