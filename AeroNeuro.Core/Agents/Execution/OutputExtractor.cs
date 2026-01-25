namespace AeroNeuro.Core.Agents.Execution;

public class OutputExtractor : IOutputExtractor<byte, byte[]>
{
    private readonly int _actionSize;

    public OutputExtractor(int actionSize)
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
