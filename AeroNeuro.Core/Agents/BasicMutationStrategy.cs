using AeroNeuro.Common;

namespace AeroNeuro.Core.Agents;

public class BasicMutationStrategy : IMutationStrategy<ushort[]>
{
    public void Mutate(ushort[] data)
    {
        // A very basic mutation: flip a random bit in a random ushort.
        if (data == null || data.Length == 0)
            return;

        int randomIndex = ThreadSafeRandom.Instance.Next(data.Length);
        int randomBit = 1 << ThreadSafeRandom.Instance.Next(16);
        data[randomIndex] = (ushort)(data[randomIndex] ^ randomBit);
    }
}