using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Agents.Execution;
using AeroNeuro.Core.Agents.Mutation;
using AeroNeuro.Core.Agents.Persistence;

namespace AeroNeuro.Core.Agents.Implementations.GenomeAgent;

public class GenomeAgentPersistence : IAgentPersistence<byte>
{
    private readonly IMutationStrategy<ushort[]> _mutationStrategy;
    private readonly IOutputExtractor<byte, byte[]> _outputExtractor;
    private readonly IProgramExecutor<byte, ushort[]> _programExecutor;

    public GenomeAgentPersistence(
        IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor)
    {
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
    }

    /// <inheritdoc/>
    public void SaveAgent(string path, IAgent<byte> agent)
    {
        var data = agent.GetAgentData();
        if (data is not GenomeAgentData agentData)
        {
            throw new ArgumentException($"Agent data is not compatible with {nameof(GenomeAgentData)}.", nameof(agent));
        }

        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = System.Text.Json.JsonSerializer.Serialize(agentData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    /// <inheritdoc/>
    public IAgent<byte> LoadAgent(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The specified agent file was not found.", path);
        }

        string json = File.ReadAllText(path);
        GenomeAgentData? agentData = System.Text.Json.JsonSerializer.Deserialize<GenomeAgentData>(json);

        if (agentData == null)
        {
            throw new InvalidOperationException($"Failed to deserialize agent data from {path}.");
        }

        return new GenomeAgent(
            _mutationStrategy,
            _outputExtractor,
            _programExecutor,
            agentData);
    }
}