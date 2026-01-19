namespace AeroNeuro.Core.Environments;

/// <summary>
/// Interface for an environment that agents can interact with. It could be a
/// simulation or real-world setup, a game, or any scenario where agents perceive 
/// observations and take actions to achieve goals.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Size of the observation vector provided to agents.
    /// </summary>
    int ObservationSize { get; }

    /// <summary>
    /// Size of the action vector expected from agents.
    /// </summary>
    int ActionSize { get; }

    /// <summary>
    /// Get the current observation from the environment.
    /// </summary>
    float[] GetObservation();

    /// <summary>
    /// Apply the given actions to the environment and advance its state.
    /// Returns the reward obtained after taking the actions.
    /// </summary>
    float Step(float[] actions);

    /// <summary>
    /// Indicates whether the current episode has ended.
    /// </summary>
    bool IsDone { get; }

    /// <summary>
    /// Reset the environment to its initial state.
    /// </summary>
    void Reset();
}
