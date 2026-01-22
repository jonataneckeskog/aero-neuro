namespace AeroNeuro.Core.Agents;

public interface IOutputExtractor<T, K>
{
    T[] ExtractOutput(K program);
}