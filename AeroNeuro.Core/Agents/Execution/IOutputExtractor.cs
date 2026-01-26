namespace AeroNeuro.Core.Agents.Execution;

public interface IOutputExtractor<T, K>
{
    T[] ExtractOutput(K program);
}