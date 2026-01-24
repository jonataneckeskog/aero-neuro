namespace AeroNeuro.Core.Agents;

public interface IMutationStrategy<T>
{
    void Mutate(T data);
}