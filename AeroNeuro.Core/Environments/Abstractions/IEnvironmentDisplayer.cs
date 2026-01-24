namespace AeroNeuro.Core.Environments;

/// <summary>
/// Interface for displaying environments to the user.
/// </summary>
public interface IEnvironmentDisplayer<T>
{
    /// <summary>
    /// Displays the specified environment at it's current state.
    /// </summary>
    void DisplayEnvironment(IEnvironment<T> environment);
}
