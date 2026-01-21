namespace AeroNeuro.Core.Agents;

public class DoubleNetworkAgent : IAgent<byte>
{
    private readonly int _input;
    private readonly int _output;

    private ushort[] _program;
    private ushort[] _genome;
    private byte[] _memory;

    public DoubleNetworkAgent(int input, int output, int NetworkSize = 32, int MemorySize = 64)
    {
        _input = input;
        _output = output;
        _program = new ushort[NetworkSize];
        _genome = new ushort[NetworkSize];
        _memory = new byte[MemorySize];
    }

    private DoubleNetworkAgent(int input, int output, ushort[] program, ushort[] genome, int memorySize)
    {
        _input = input;
        _output = output;
        _program = program;
        _genome = genome;
        _memory = new byte[memorySize];
    }

    /// <inheritdoc/>
    public byte[] Decide(byte[] observations)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IAgent<byte> Clone()
    {
        return new DoubleNetworkAgent(_input, _output,
                (ushort[])_program.Clone(), (ushort[])_genome.Clone(), _memory.Length);
    }

    /// <inheritdoc/>
    public void Mutate()
    {
        throw new NotImplementedException();
    }
}