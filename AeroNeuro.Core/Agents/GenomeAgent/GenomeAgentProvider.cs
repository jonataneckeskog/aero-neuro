using AeroNeuro.Core.Environments;
using AeroNeuro.Common;

namespace AeroNeuro.Core.Agents.GenomeAgent;

public class GenomeAgentProvider : IAgentProvider<byte>
{
    private readonly IEnvironment<byte> _environment;
    private readonly IMutationStrategy<ushort[]> _mutationStrategy;
    private readonly IOutputExtractor<byte, byte[]> _outputExtractor;
    private readonly IProgramExecutor<byte, ushort[]> _programExecutor;
    private readonly int _networkSize;
    private readonly int _memorySize;


    public GenomeAgentProvider(IEnvironment<byte> environment,
        IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor,
        int networkSize = 32, int memorySize = 32)
    {
        _environment = environment;
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
        _networkSize = networkSize;
        _memorySize = memorySize;
    }

    /// <inheritdoc/>
    public IAgent<byte> CreateBaseAgent()
    {
        return new GenomeAgent(
            _mutationStrategy,
            _outputExtractor,
            _programExecutor,
            _environment.ObservationSize,
            _networkSize,
            _memorySize);
    }

    /// <inheritdoc/>
    public IAgent<byte> CreateRandomAgent(int minNodes, int maxNodes)
    {
        int networkSize = ThreadSafeRandom.Instance.Next(minNodes, maxNodes);

        IAgent<byte> agent = new GenomeAgent(
            _mutationStrategy,
            _outputExtractor,
            _programExecutor,
            _environment.ObservationSize,
            networkSize: networkSize,
            memorySize: Math.Max(networkSize, _environment.ObservationSize));

        agent.Mutate();

        return agent;
    }
}