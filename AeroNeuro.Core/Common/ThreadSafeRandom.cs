namespace AeroNeuro.Common;

public static class ThreadSafeRandom
{
    [ThreadStatic]
    private static Random? _local;

    public static Random Instance => _local ??= new Random();
}