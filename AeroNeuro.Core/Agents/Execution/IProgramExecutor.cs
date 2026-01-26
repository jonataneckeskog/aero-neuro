using System.Numerics;

namespace AeroNeuro.Core.Agents.Execution;

public interface IProgramExecutor<T, K> where T : INumber<T>
{
    /// <summary>
    /// Executes the provided program using the given memory bank.
    /// </summary>
    /// <param name="memory">The working memory (inputs are pre-loaded here).</param>
    /// <param name="program">The instruction set to run.</param>
    void Execute(T[] memory, K program);
}
