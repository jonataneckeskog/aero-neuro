using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Training.Decorators;

public class DiversityDecorator<T> : IEnvironment<T>
{
    private readonly IEnvironment<T> _environment;
    private readonly IPopulationProvider<T> _populationProvider;

    // We only care about Crowding. 
    private const float CrowdingPenaltyWeight = 0.5f;
    private const float CrowdingRadius = 0.1f;

    private readonly int[] _buckets;
    private readonly int _bucketCount;

    public DiversityDecorator(IEnvironment<T> environment, IPopulationProvider<T> populationProvider)
    {
        _environment = environment;
        _populationProvider = populationProvider;

        // Create buckets to map the Action Space (0.0 to 1.0)
        _bucketCount = (int)Math.Ceiling(1.0f / CrowdingRadius);
        _buckets = new int[_bucketCount];
    }

    /// <inheritdoc/>
    public int ObservationSize => _environment.ObservationSize;

    /// <inheritdoc/>
    public int ActionSize => _environment.ActionSize;

    /// <inheritdoc/>
    public bool IsDone => _environment.IsDone;

    public T[] GetObservation() => _environment.GetObservation();

    /// <inheritdoc/>
    public void Reset()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
        _environment.Reset();
    }

    /// <inheritdoc/>
    public void Act(T[] actions)
    {
        // 1. Calculate the "Strategy Hash" (Location) from the actions themselves
        double strategyLocation = CalculateStrategyHash(actions);

        // 2. Identify Bucket (O(1))
        int bucketIdx = GetBucketIndex(strategyLocation);

        // 3. Register presence
        _buckets[bucketIdx]++;

        _environment.Act(actions);
    }

    /// <inheritdoc/>
    public float Step(T[] actions)
    {
        float baseReward = _environment.Step(actions);

        // 1. Re-calculate location
        double strategyLocation = CalculateStrategyHash(actions);
        int bucketIdx = GetBucketIndex(strategyLocation);

        // 2. Calculate Crowding Penalty
        // How many OTHER agents are doing the exact same thing?
        int neighbors = Math.Max(0, _buckets[bucketIdx] - 1);

        float crowdingPenalty = neighbors * CrowdingPenaltyWeight;

        return baseReward - crowdingPenalty;
    }

    public void ClearBuckets()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
    }

    /// <summary>
    /// Maps a complex action vector to a single scalar [0, 1] representing the "Strategy".
    /// </summary>
    private double CalculateStrategyHash(T[] actions)
    {
        if (actions.Length == 0) return 0.0;

        double sum = 0;
        // We use a simple average of the absolute values to determine "Intensity"
        // This groups "Passive" agents (0.0) vs "Active" agents (1.0)
        for (int i = 0; i < actions.Length; i++)
        {
            sum += Math.Abs(Convert.ToDouble(actions[i]));
        }

        double average = sum / actions.Length;

        // Ensure result is strictly [0, 1] for the bucket logic
        return Math.Clamp(average, 0.0, 1.0);
    }

    private int GetBucketIndex(double location)
    {
        // Map 0.0-1.0 to 0-(BucketCount-1)
        int idx = (int)(location * (_bucketCount - 1));
        return Math.Clamp(idx, 0, _bucketCount - 1); // Safety clamp
    }
}