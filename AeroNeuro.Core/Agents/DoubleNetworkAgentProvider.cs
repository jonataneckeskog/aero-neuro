using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Agents;

public class DoubleNetworkAgentProvider : IAgentProvider<byte>
{
    private readonly IEnvironment<byte> _environment;

    public DoubleNetworkAgentProvider(IEnvironment<byte> environment)
    {
        _environment = environment;
    }

    public IAgent<byte> CreateBaseAgent()
    {
        throw new NotImplementedException();
    }

    public IAgent<byte> CreateRandomAgent(int minNodes, int maxNodes)
    {
        throw new NotImplementedException();
    }
}