using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Common;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Decorators;

public class AdversaryEvolutionTrainer<T> : IEvolutionTrainer<T>
{
    private IEvolutionTrainer<T> MainTrainer { get; }
    private IEvolutionTrainer<T> AdversaryTrainer { get; }

    public AdversaryEvolutionTrainer(IEvolutionTrainer<T> mainTrainer, IEvolutionTrainer<T> adversaryTrainer)
    {
        MainTrainer = mainTrainer;
        AdversaryTrainer = adversaryTrainer;
    }

    /// <inheritdoc/>
    public void EvolveGeneration()
    {
        MainTrainer.EvolveGeneration();
        AdversaryTrainer.EvolveGeneration();
    }

    /// <inheritdoc/>
    public List<IAgent<T>> GetBestAgents(int count)
    {
        return MainTrainer.GetBestAgents(count);
    }

    /// <inheritdoc/>
    public EvolutionStats GetStats()
    {
        return MainTrainer.GetStats();
    }
}
