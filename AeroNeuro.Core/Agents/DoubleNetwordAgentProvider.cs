using AeroNeuro.Core.Environments;

namespace AeroNeuro.Core.Agents;

public class DoubleNetworkAgentProvider : IAgentProvider
{
    private readonly IEnvironment _environment;

    public DoubleNetworkAgentProvider(IEnvironment environment)
    {
        _environment = environment;
    }

    public IAgent CreateBaseAgent()
    {
        throw new NotImplementedException();
    }

    public IAgent CreateRandomAgent(int minNodes, int maxNodes)
    {
        throw new NotImplementedException();
    }
}