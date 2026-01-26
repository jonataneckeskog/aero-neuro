using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Agents.Execution;
using AeroNeuro.Core.Agents.Mutation;
using AeroNeuro.Core.Common;
using System.Numerics;

namespace AeroNeuro.Core.Agents.Implementations.GenomeAgent;

public class GenomeAgentProvider : IAgentProvider<byte>
{
    private readonly AgentTopology _agentTopology;
    private readonly IMutationStrategy<ushort[]> _mutationStrategy;
    private readonly IOutputExtractor<byte, byte[]> _outputExtractor;
    private readonly IProgramExecutor<byte, ushort[]> _programExecutor;
    private readonly int _networkSize;
    private readonly int _memorySize;
    private readonly int _maxNetworkSize;

    public GenomeAgentProvider(AgentTopology agentTopology,
        IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor,
        int maxNetworkSize, int networkSize = 32, int memorySize = 32)
    {
        _agentTopology = agentTopology;
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
            _agentTopology.InputCount,
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
            _agentTopology.InputCount,
            networkSize: networkSize,
            memorySize: (int)BitOperations.RoundUpToPowerOf2(
                (uint)Math.Max(networkSize, Math.Max(_agentTopology.InputCount,
                _agentTopology.OutputCount))),
            maxNetworkSize: _maxNetworkSize
            );

        agent.Mutate();

        return agent;
    }
}