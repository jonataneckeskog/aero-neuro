using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Execution;
using AeroNeuro.Common;

namespace AeroNeuro.Core.Agents;

public class GenomeNetworkAgentProvider : IAgentProvider<byte>
{
    private readonly IEnvironment<byte> _environment;

    public GenomeNetworkAgentProvider(IEnvironment<byte> environment)
    {
        _environment = environment;
    }

    public IAgent<byte> CreateBaseAgent()
    {
        IMutationStrategy<ushort[]> mutationStrategy = new BasicMutationStrategy();
        IOutputExtractor<byte, byte[]> outputExtractor = new OutputExtractor(_environment.ActionSize);
        IProgramExecutor<byte, ushort[]> programExecutor = new BasicProgramExecutor();

        return new GenomeNetworkAgent(
            mutationStrategy,
            outputExtractor,
            programExecutor,
            _environment.ObservationSize);
    }

    public IAgent<byte> CreateRandomAgent(int minNodes, int maxNodes)
    {
        IMutationStrategy<ushort[]> mutationStrategy = new BasicMutationStrategy();
        IOutputExtractor<byte, byte[]> outputExtractor = new OutputExtractor(_environment.ActionSize);
        IProgramExecutor<byte, ushort[]> programExecutor = new BasicProgramExecutor();
        int networkSize = ThreadSafeRandom.Instance.Next(minNodes, maxNodes);

        IAgent<byte> agent = new GenomeNetworkAgent(
            mutationStrategy,
            outputExtractor,
            programExecutor,
            _environment.ObservationSize,
            networkSize: networkSize);

        agent.Mutate();

        return agent;
    }
}