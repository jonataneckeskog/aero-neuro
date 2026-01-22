namespace AeroNeuro.Core.Agents.GenomeAgent;

public class GenomeAgentData : AgentData
{
    public override string Type => "GenomeNetworkAgent";
    public ushort[] Genome { get; set; } = [];
    public int InputSize { get; set; }
    public int MemorySize { get; set; }
    public int NetworkSize { get; set; }
    public int MaxNetworkSize { get; set; }
}