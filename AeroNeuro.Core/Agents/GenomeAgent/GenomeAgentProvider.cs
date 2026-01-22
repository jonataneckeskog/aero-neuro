using AeroNeuro.Core.Environments;
using AeroNeuro.Common;
using System.Numerics;

namespace AeroNeuro.Core.Agents.GenomeAgent;

public class GenomeAgentProvider : IAgentProvider<byte>
{
    private readonly IEnvironment<byte> _environment;
    private readonly IMutationStrategy<ushort[]> _mutationStrategy;
    private readonly IOutputExtractor<byte, byte[]> _outputExtractor;
    private readonly IProgramExecutor<byte, ushort[]> _programExecutor;
    private readonly int _networkSize;
    private readonly int _memorySize;
    private readonly int _maxNetworkSize;

    public GenomeAgentProvider(IEnvironment<byte> environment,
        IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor,
        int maxNetworkSize, int networkSize = 32, int memorySize = 32)
    {
        _environment = environment;
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
        _networkSize = networkSize;
        _memorySize = memorySize;
        _maxNetworkSize = maxNetworkSize;
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
            _memorySize,
            _maxNetworkSize
            );
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
            memorySize: (int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(networkSize, _environment.ObservationSize)),
            maxNetworkSize: _maxNetworkSize
            );

        agent.Mutate();

        return agent;
    }
}