namespace AeroNeuro.Core.Agents
{
    /// <summary>
    /// Interface for observing training state and statistics during evolution.
    /// </summary>
    public interface ITrainingStateProvider
    {
        /// <summary>
        /// Gets the current generation count.
        /// </summary>
        /// <returns>The current generation count.</returns>
        int GetGenerationCount();

        /// <summary>
        /// Gets the best fitness value observed so far.
        /// </summary>
        /// <returns>The best fitness value.</returns>
        float GetBestFitness();

        /// <summary>
        /// Gets the average fitness of the current generation.
        /// </summary>
        /// <returns>The average fitness value.</returns>
        float GetAverageFitness();

        /// <summary>
        /// Gets the worst fitness of the current generation.
        /// </summary>
        /// <returns>The worst fitness value.</returns>
        float GetWorstFitness();

        /// <summary>
        /// Gets the duration of the current generation.
        /// </summary>
        /// <returns>The generation duration.</returns>
        TimeSpan GetGenerationDuration();
    }
}