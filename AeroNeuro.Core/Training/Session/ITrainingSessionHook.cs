namespace AeroNeuro.Core.Training.Session;

public interface ITrainingSessionHook<T>
{
    /// <summary>
    /// Called once before the training loop begins. 
    /// Useful for initializing resources (files, DB connections).
    /// </summary>
    void OnSessionStart(TrainingContext<T> context) { }

    /// <summary>
    /// Called at the beginning of a generation, before evaluation.
    /// Useful for resetting environment variables or dynamic difficulty adjustment.
    /// </summary>
    void OnGenerationStart(TrainingContext<T> context) { }

    /// <summary>
    /// Called after a generation has been evolved.
    /// Useful for logging, saving checkpoints, or checking stopping conditions.
    /// </summary>
    void OnGenerationEnd(TrainingContext<T> context) { }

    /// <summary>
    /// Called once after the training loop finishes (or is aborted).
    /// Useful for cleanup.
    /// </summary>
    void OnSessionEnd(TrainingContext<T> context) { }
}