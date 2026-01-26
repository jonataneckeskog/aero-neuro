namespace AeroNeuro.Core.Agents.Execution;

public class MemoryOutputExtractor : IOutputExtractor<byte, byte[]>
{
    private readonly int _actionSize;

    public MemoryOutputExtractor(int actionSize)
    {
        _actionSize = actionSize;
    }

    public byte[] ExtractOutput(byte[] memory)
    {
        byte[] output = new byte[_actionSize];
        int bytesToCopy = Math.Min(memory.Length, _actionSize);
        Array.Copy(memory, 0, output, 0, bytesToCopy);
        return output;
    }
}
