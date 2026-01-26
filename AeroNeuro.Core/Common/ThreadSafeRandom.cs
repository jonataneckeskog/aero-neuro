namespace AeroNeuro.Core.Common;

/// <summary>
/// Share a Random instance across files and threads.
/// </summary>
public static class ThreadSafeRandom
{
    [ThreadStatic]
    private static Random? _local;

    public static Random Instance => _local ??= new Random();
}