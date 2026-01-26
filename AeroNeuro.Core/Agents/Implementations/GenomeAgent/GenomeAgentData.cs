using AeroNeuro.Core.Agents.Abstractions;

namespace AeroNeuro.Core.Agents.Implementations.GenomeAgent;

public class GenomeAgentData : AgentData
{
    public override string Type => "GenomeNetworkAgent";
    public ushort[] Program { get; set; } = [];
    public ushort[] Genome { get; set; } = [];
    public int MemorySize { get; set; }
    public int GenomeSize { get; set; }
    public int NetworkSize { get; set; }
    public int MaxNetworkSize { get; set; }
}