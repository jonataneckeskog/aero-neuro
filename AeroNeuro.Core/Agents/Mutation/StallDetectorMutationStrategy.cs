using AeroNeuro.Core.Common;
using AeroNeuro.Core.Agents.Abstractions;
using System.Runtime.CompilerServices;

namespace AeroNeuro.Core.Agents.Mutation;

public class StallDetectorMutationStrategy : IMutationStrategy<ushort[]>
{
    private IEvolutionContext _evolutionObserver;
    private int _generationDelay;
    private float _fitnessThreshold;
    private int _lastGenerationChecked;
    private float _lastFitnessChecked;

    public StallDetectorMutationStrategy(IEvolutionContext evolutionObserver, int generationDelay, float fitnessThreshold)
    {
        _evolutionObserver = evolutionObserver;
        _generationDelay = generationDelay;
        _fitnessThreshold = fitnessThreshold;
        _lastGenerationChecked = evolutionObserver.GetCurrentStats().Generation;
        _lastFitnessChecked = evolutionObserver.GetCurrentStats().BestFitness;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Mutate(ushort[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var rng = ThreadSafeRandom.Instance;

        // 1. Roll 0-99. Integer math is generally faster than double precision.
        int roll = rng.Next(100);

        // 2. Map roll to flip count using a switch expression (compile-time optimized).
        // 0-69   (70 items) -> 1
        // 70-89  (20 items) -> 2
        // 90-94  (5 items)  -> 3
        // 95-97  (3 items)  -> 4
        // 98-99  (2 items)  -> 5
        int bitsToFlip = roll switch
        {
            < 70 => 1,
            < 90 => 2,
            < 95 => 3,
            < 98 => 4,
            _ => 5
        };

        float currentFitness = _evolutionObserver.GetCurrentStats().BestFitness;
        if (currentFitness - _lastFitnessChecked > _fitnessThreshold) // Reset on improvement
        {
            _lastGenerationChecked = _evolutionObserver.GetCurrentStats().Generation;
            _lastFitnessChecked = currentFitness;
        }
        else if (_evolutionObserver.GetCurrentStats().Generation - _generationDelay > _lastGenerationChecked) // Increase mutation on stall
        {
            bitsToFlip += 1;
        }

        // 3. Execute flips
        // Using a while loop is slightly lighter on the IL in some contexts than a for-loop with iterator
        while (bitsToFlip > 0)
        {
            int index = rng.Next(data.Length);

            // Generate the mask directly. 
            // Note: bit shifting is extremely cheap.
            int mask = 1 << rng.Next(16);

            data[index] ^= (ushort)mask;
            bitsToFlip--;
        }
    }
}
