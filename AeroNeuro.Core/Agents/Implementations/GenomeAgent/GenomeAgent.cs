using System.Numerics;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Agents.Execution;
using AeroNeuro.Core.Agents.Mutation;

namespace AeroNeuro.Core.Agents.Implementations.GenomeAgent;

public class GenomeAgent : IAgent<byte>
{
    IMutationStrategy<ushort[]> _mutationStrategy;
    IOutputExtractor<byte, byte[]> _outputExtractor;
    IProgramExecutor<byte, ushort[]> _programExecutor;

    private int _maxNetworkSize;
    private int _memorySize;
    private ushort[] _program;
    private ushort[] _genome;
    private byte[] _workingMemory;

    public GenomeAgent(IMutationStrategy<ushort[]> mutationStrategy,
            IOutputExtractor<byte, byte[]> outputExtractor,
            IProgramExecutor<byte, ushort[]> programExecutor,
            int inputSize, int maxNetworkSize, int networkSize, int memorySize)
    {
        _mutationStrategy = mutationStrategy;
        _programExecutor = programExecutor;
        _outputExtractor = outputExtractor;
        _program = new ushort[(int)BitOperations.RoundUpToPowerOf2((uint)networkSize * 2)];
        _genome = new ushort[networkSize];
        _maxNetworkSize = maxNetworkSize;
        _memorySize = memorySize >= inputSize ? memorySize : inputSize;
        _workingMemory = new byte[_memorySize];
    }

    public GenomeAgent(IMutationStrategy<ushort[]> mutationStrategy,
        IOutputExtractor<byte, byte[]> outputExtractor,
        IProgramExecutor<byte, ushort[]> programExecutor,
        GenomeAgentData agentData)
    {
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
        _program = (ushort[])agentData.Program.Clone();
        _genome = (ushort[])agentData.Genome.Clone();
        _maxNetworkSize = agentData.NetworkSize;
        _memorySize = agentData.MemorySize;
        _workingMemory = new byte[_memorySize];
    }

    private GenomeAgent(IMutationStrategy<ushort[]> mutationStrategy,
            IOutputExtractor<byte, byte[]> outputExtractor,
            IProgramExecutor<byte, ushort[]> programExecutor, int maxNetworkSize,
            ushort[] program, ushort[] genome, int memorySize)
    {
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
        _program = program;
        _genome = genome;
        _maxNetworkSize = maxNetworkSize;
        _memorySize = memorySize;
        _workingMemory = new byte[_memorySize];
        _maxNetworkSize = program.Length;
    }

    /// <inheritdoc/>
    public byte[] Decide(byte[] observations)
    {
        // Initialize and load memory
        Array.Clear(_workingMemory, 0, _workingMemory.Length);
        int bytesToCopy = Math.Min(observations.Length, _memorySize);
        Array.Copy(observations, 0, _workingMemory, 0, bytesToCopy);

        // Execute program
        _programExecutor.Execute(_workingMemory, _program);

        // Generate output
        return _outputExtractor.ExtractOutput(_workingMemory);
    }

    /// <inheritdoc/>
    public IAgent<byte> Clone()
    {
        return new GenomeAgent(_mutationStrategy, _outputExtractor, _programExecutor, _maxNetworkSize,
            (ushort[])_program.Clone(), (ushort[])_genome.Clone(), _memorySize);
    }

    /// <inheritdoc/>
    public void Mutate()
    {
        _mutationStrategy.Mutate(_genome);
        Array.Clear(_program, 0, _program.Length);
        Array.Copy(_genome, _program, _genome.Length);
    }

    /// <inheritdoc/>
    public AgentData GetAgentData()
    {
        return new GenomeAgentData
        {
            Program = (ushort[])_program.Clone(),
            Genome = (ushort[])_genome.Clone(),
            MemorySize = _memorySize,
            GenomeSize = _genome.Length,
            NetworkSize = _program.Length,
            MaxNetworkSize = _maxNetworkSize
        };
    }
}
