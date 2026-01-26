using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Training.Selection;

/// <summary>
/// Selects top performers by fitness along with statistical outliers from the lower fitness range.
/// This strategy preserves genetic diversity by including agents that are unusual despite lower fitness.
/// </summary>
public class OutlierPopulationSelector<T> : IPopulationSelector<T>
{
    private readonly float _topFraction;
    private readonly float _outlierThreshold;
    private readonly int _minAgents;
    private readonly int _maxAgents;

    /// <summary>
    /// Initializes a new instance of the OutlierPopulationSelector.
    /// </summary>
    /// <param name="topFraction">Fraction of the population to select purely based on top rank (0.0 to 1.0). Default 0.1 (10%).</param>
    /// <param name="outlierThreshold">IQR multiplier. Higher = stricter outlier definition. (Default: 1.5).</param>
    /// <param name="minAgents">The minimum number of agents to select (prevents population collapse).</param>
    /// <param name="maxAgents">The maximum number of agents to select (prevents processing bloat).</param>
    public OutlierPopulationSelector(
        float topFraction = 0.05f,
        float outlierThreshold = 1.5f,
        int minAgents = 5,
        int maxAgents = 20)
    {
        if (topFraction < 0f || topFraction > 1f) throw new ArgumentOutOfRangeException(nameof(topFraction), "Top fraction must be between 0 and 1.");
        if (outlierThreshold <= 0f) throw new ArgumentOutOfRangeException(nameof(outlierThreshold), "Outlier threshold must be positive.");
        if (minAgents < 1) throw new ArgumentOutOfRangeException(nameof(minAgents), "Min agents must be at least 1.");
        if (maxAgents < minAgents) throw new ArgumentException("Max agents cannot be less than min agents.", nameof(maxAgents));

        _topFraction = topFraction;
        _outlierThreshold = outlierThreshold;
        _minAgents = minAgents;
        _maxAgents = maxAgents;
    }

    /// <inheritdoc/>
    public List<IAgent<T>> SelectPopulation(List<(float Fitness, IAgent<T> Agent)> evaluatedPopulation)
    {
        int populationCount = evaluatedPopulation.Count;

        if (populationCount == 0) return [];

        // 1. Determine how many "Elites" to take based on fraction
        int topCount = (int)Math.Ceiling(populationCount * _topFraction);

        // Ensure we don't exceed population or maxAgents immediately
        topCount = Math.Clamp(topCount, 0, Math.Min(populationCount, _maxAgents));

        // Start the selection list
        var selectedAgents = new List<(float Fitness, IAgent<T> Agent)>();

        // Add the Guaranteed Top Tier
        var topTier = evaluatedPopulation.Take(topCount).ToList();
        selectedAgents.AddRange(topTier);

        // 2. Identify Outliers from the *Remaining* pool
        var remainingPool = evaluatedPopulation.Skip(topCount).ToList();

        if (remainingPool.Count > 0 && selectedAgents.Count < _maxAgents)
        {
            // Pass the specific threshold instance variable
            var outliers = FindOutliers(remainingPool, _outlierThreshold);

            // Add outliers, respecting MaxAgents
            foreach (var outlier in outliers)
            {
                if (selectedAgents.Count >= _maxAgents) break;
                selectedAgents.Add(outlier);
            }
        }

        // 3. Enforce MinAgents (Fill with next best if we are short)
        // We look at the remaining pool again to find non-outlier agents that are simply "next best"
        if (selectedAgents.Count < _minAgents && selectedAgents.Count < populationCount)
        {
            // Create a set for O(1) lookup to avoid adding duplicates we already picked as outliers
            var currentlySelectedIds = selectedAgents.Select(x => x.Agent).ToHashSet();

            foreach (var candidate in remainingPool)
            {
                if (selectedAgents.Count >= _minAgents) break;

                if (!currentlySelectedIds.Contains(candidate.Agent))
                {
                    selectedAgents.Add(candidate);
                    currentlySelectedIds.Add(candidate.Agent);
                }
            }
        }

        return selectedAgents.Select(x => x.Agent).ToList();
    }

    /// <summary>
    /// Identifies outliers using the Interquartile Range (IQR) method.
    /// </summary>
    private static List<(float Fitness, IAgent<T> Agent)> FindOutliers(
        List<(float Fitness, IAgent<T> Agent)> agentsDescending,
        float threshold)
    {
        if (agentsDescending.Count < 4) return [];

        // Reverse to ascending order for percentile calculation
        var fitnesses = agentsDescending.Select(x => x.Fitness).Reverse().ToList();

        float q1 = Percentile(fitnesses, 0.25f);
        float q3 = Percentile(fitnesses, 0.75f);
        float iqr = q3 - q1;

        // Use the passed threshold, not a hardcoded value
        float lowerBound = q1 - (iqr * threshold);
        float upperBound = q3 + (iqr * threshold);

        return agentsDescending
            .Where(x => x.Fitness < lowerBound || x.Fitness > upperBound)
            .ToList();
    }

    private static float Percentile(List<float> sortedValues, float percentile)
    {
        if (sortedValues.Count == 0) return 0f;

        float index = (sortedValues.Count - 1) * percentile;
        int lower = (int)index;
        int upper = lower + 1;

        if (upper >= sortedValues.Count) return sortedValues[lower];

        float weight = index - lower;
        return sortedValues[lower] * (1 - weight) + sortedValues[upper] * weight;
    }
}
