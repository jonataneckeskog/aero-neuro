using AeroNeuro.Core.Environments.Abstractions;
using AeroNeuro.Core.Training.Abstractions;

namespace AeroNeuro.Core.Environments.Decorators;

public class LocationDecorator<T> : IEnvironment<T>
{
    private readonly IEnvironment<T> _environment;
    private readonly IPopulationProvider<T> _populationProvider;

    private const float LocationWeight = 1.0f;
    private const float CrowdingPenaltyWeight = 0.5f;
    private const float CrowdingRadius = 0.1f;

    // We divide the 0.0 to 1.0 range into buckets.
    // Number of buckets = 1 / Radius. If Radius is 0.1, we have 10 buckets.
    private readonly int[] _buckets;
    private readonly int _bucketCount;

    /// <inheritdoc/>
    public int ObservationSize => _environment.ObservationSize;

    /// <inheritdoc/>
    public int ActionSize => _environment.ActionSize + 1;

    /// <inheritdoc/>
    public bool IsDone => _environment.IsDone;

    public LocationDecorator(IEnvironment<T> environment, IPopulationProvider<T> populationProvider)
    {
        _environment = environment;
        _populationProvider = populationProvider;

        _bucketCount = (int)Math.Ceiling(1.0f / CrowdingRadius);
        _buckets = new int[_bucketCount];
    }

    /// <inheritdoc/>
    public void Reset()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
        _environment.Reset();
    }

    /// <inheritdoc/>
    public void Act(T[] actions)
    {
        // 1. Get location and normalize/clamp to [0, 1]
        int locationIndex = _environment.ActionSize;
        double location = Math.Clamp(Convert.ToDouble(actions[locationIndex]), 0.0, 1.0);

        // 2. Identify which bucket this agent falls into (O(1))
        int bucketIdx = (int)(location * (_bucketCount - 1));

        // 3. Increment bucket count (O(1))
        _buckets[bucketIdx]++;

        _environment.Act(actions);
    }

    /// <inheritdoc/>
    public float Step(T[] actions)
    {
        float baseReward = _environment.Step(actions);

        int locationIndex = _environment.ActionSize;
        double agentLocation = Math.Clamp(Convert.ToDouble(actions[locationIndex]), 0.0, 1.0);

        // A: Location Bonus (O(1))
        float distToTarget = (float)Math.Abs(1.0 - agentLocation);
        float locationBonus = (1.0f - distToTarget) * LocationWeight;

        // B: Crowding Penalty (O(1))
        // We look at the agent's bucket and potentially immediate neighbors
        int bucketIdx = (int)(agentLocation * (_bucketCount - 1));

        // Crowding is simply the number of agents in the same bucket
        // Subtract 1 to exclude the current agent itself
        int neighbors = Math.Max(0, _buckets[bucketIdx] - 1);

        float crowdingPenalty = neighbors * CrowdingPenaltyWeight;

        return baseReward + locationBonus - crowdingPenalty;
    }

    // This must be called by the Orchestrator/Evaluator at the end of every "Step" loop
    // to clear the map for the next physics tick.
    public void ClearBuckets()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
    }

    public T[] GetObservation()
    {
        throw new NotImplementedException();
    }
}