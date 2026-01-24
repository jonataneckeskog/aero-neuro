namespace AeroNeuro.Core.Agents;

/// <summary>
/// Interface for persisting and loading agent data.
/// </summary>
public interface IAgentPersistence<T>
{
    /// <summary>
    /// Loads an agent from the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IAgent<T> LoadAgent(string path);

    /// <summary>
    /// Saves an agent to the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    void SaveAgent(string path, IAgent<T> data);
}
