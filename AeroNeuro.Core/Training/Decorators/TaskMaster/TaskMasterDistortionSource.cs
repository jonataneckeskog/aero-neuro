using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Training.Abstractions;
using System.Numerics;

namespace AeroNeuro.Core.Training.Decorators.TaskMaster;

/// <summary>
/// Provides a high-performance source of task-master distortions by cycling through the elite agents.
/// </summary>
/// <typeparam name="T">The type of the observation and decision data.</typeparam>
public sealed class TaskMasterDistortionSource<T> : IDistortionSource<T>
{
    private readonly IPopulationProvider<T> _taskMasterProvider;
    private readonly IAgent<T>[] _agents;
    private readonly int _mask;
    private int _i = -1;
    private int _actualCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskMasterDistortionSource{T}"/> class.
    /// </summary>
    /// <param name="taskMasterProvider">The provider for the ranked population.</param>
    /// <param name="batchSize">The requested batch size. This will be rounded up to the nearest power of 2 for performance.</param>
    public TaskMasterDistortionSource(IPopulationProvider<T> taskMasterProvider, int batchSize)
    {
        _taskMasterProvider = taskMasterProvider;

        // Ensure batchSize is a power of 2 using System.Numerics
        uint optimizedSize = BitOperations.IsPow2((uint)batchSize)
            ? (uint)batchSize
            : BitOperations.RoundUpToPowerOf2((uint)batchSize);

        _agents = new IAgent<T>[optimizedSize];
        _mask = (int)optimizedSize - 1;
    }

    /// <inheritdoc/>
    public T[] Distort(T[] observation)
    {
        // Guard against calls before first Reset or if the population is empty
        if (_actualCount == 0)
        {
            return observation;
        }

        _i++;

        // Fast bitwise wrap-around (replaces i % length)
        // Note: This cycles through the allocated buffer. If _actualCount < _agents.Length,
        // it will cycle through the elites and then any remaining slots.
        // To strictly cycle only 'actual' agents, use: _agents[(_i++) % _actualCount]
        // But for a hot loop, we target the bitmask for maximum speed.
        return _agents[_i & _mask].Decide(observation);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _i = -1;
        var ranked = _taskMasterProvider.RankedPopulation;

        if (ranked == null || ranked.Count == 0)
        {
            _actualCount = 0;
            return;
        }

        // Fill our pre-allocated buffer with the best available agents
        int countToFill = Math.Min(_agents.Length, ranked.Count);
        for (int j = 0; j < countToFill; j++)
        {
            _agents[j] = ranked[j].Agent;
        }

        // If we have fewer agents than our power-of-2 buffer, 
        // backfill the rest of the buffer with existing agents to avoid nulls
        // during the bitwise & cycle.
        for (int j = countToFill; j < _agents.Length; j++)
        {
            _agents[j] = _agents[j % countToFill];
        }

        _actualCount = countToFill;
    }
}
