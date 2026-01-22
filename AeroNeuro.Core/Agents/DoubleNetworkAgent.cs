using AeroNeuro.Core.Execution;

namespace AeroNeuro.Core.Agents;

public class DoubleNetworkAgent : IAgent<byte>
{
    IMutationStrategy<ushort[]> _mutationStrategy;
    IOutputExtractor<byte, byte[]> _outputExtractor;
    IProgramExecutor<byte, ushort[]> _programExecutor;

    private int _inputSize;
    private int _memorySize;
    private ushort[] _program;
    private ushort[] _genome;

    public DoubleNetworkAgent(IMutationStrategy<ushort[]> mutationStrategy,
            IOutputExtractor<byte, byte[]> outputExtractor,
            IProgramExecutor<byte, ushort[]> programExecutor,
            int inputSize, int networkSize = 32, int memorySize = 64)
    {
        _mutationStrategy = mutationStrategy;
        _programExecutor = programExecutor;
        _outputExtractor = outputExtractor;
        _program = new ushort[networkSize];
        _genome = new ushort[networkSize];
        _inputSize = inputSize;
        _memorySize = memorySize >= inputSize ? memorySize : inputSize;
    }

    private DoubleNetworkAgent(IMutationStrategy<ushort[]> mutationStrategy,
            IOutputExtractor<byte, byte[]> outputExtractor,
            IProgramExecutor<byte, ushort[]> programExecutor,
            int inputSize, ushort[] program, ushort[] genome, int memorySize)
    {
        _mutationStrategy = mutationStrategy;
        _outputExtractor = outputExtractor;
        _programExecutor = programExecutor;
        _program = program;
        _genome = genome;
        _inputSize = inputSize;
        _memorySize = memorySize;
    }

    /// <inheritdoc/>
    public byte[] Decide(byte[] observations)
    {
        // Initialize and load memory
        byte[] memory = new byte[_memorySize];
        int bytesToCopy = Math.Min(observations.Length, _memorySize);
        Array.Copy(observations, 0, memory, 0, bytesToCopy);

        // Execute program
        _programExecutor.Execute(memory, _program);

        // Generate output
        return _outputExtractor.ExtractOutput(memory);
    }

    /// <inheritdoc/>
    public IAgent<byte> Clone()
    {
        return new DoubleNetworkAgent(_mutationStrategy, _outputExtractor, _programExecutor, _inputSize,
                (ushort[])_program.Clone(), (ushort[])_genome.Clone(), _memorySize);
    }

    /// <inheritdoc/>
    public void Mutate()
    {
        _mutationStrategy.Mutate(_genome);
    }
}