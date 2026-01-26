namespace AeroNeuro.Core.Agents.Mutation;

public interface IMutationStrategy<T>
{
    void Mutate(T data);
}