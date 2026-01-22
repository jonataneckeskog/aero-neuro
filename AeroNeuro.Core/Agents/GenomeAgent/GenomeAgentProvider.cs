using AeroNeuro.Core.Environments;
using AeroNeuro.Common;

namespace AeroNeuro.Core.Agents.GenomeAgent;

public class GenomeAgentProvider : IAgentProvider<byte>
{
    private readonly IEnvironment<byte> _environment;
    private readonly IMutationStrategy<ushort[]> _mutationStrategy;
    private readonly IOutputExtractor<byte, byte[]> _outputExtractor;
    private readonly IProgramExecutor<byte, ushort[]> _programExecutor;

    public GenomeAgentProvider(IEnvironment<byte> environment,
        IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor)
    {
        _environment = environment;
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
    }

    public IAgent<byte> CreateBaseAgent()
    {
        return new GenomeAgent(
            _mutationStrategy,
            _outputExtractor,
            _programExecutor,
            _environment.ObservationSize);
    }

    public IAgent<byte> CreateRandomAgent(int minNodes, int maxNodes)
    {
        int networkSize = ThreadSafeRandom.Instance.Next(minNodes, maxNodes);

        IAgent<byte> agent = new GenomeAgent(
            _mutationStrategy,
            _outputExtractor,
            _programExecutor,
            _environment.ObservationSize,
            networkSize: networkSize);

        agent.Mutate();

        return agent;
    }
}