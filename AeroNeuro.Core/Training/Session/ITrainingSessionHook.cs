namespace AeroNeuro.Core.Training.Session;

public interface ITrainingSessionHook
{
    /// <summary>
    /// Called once before the training loop begins. 
    /// Useful for initializing resources (files, DB connections).
    /// </summary>
    void OnSessionStart(TrainingContext context) { }

    /// <summary>
    /// Called at the beginning of a generation, before evaluation.
    /// Useful for resetting environment variables or dynamic difficulty adjustment.
    /// </summary>
    void OnGenerationStart(TrainingContext context) { }

    /// <summary>
    /// Called after a generation has been evolved.
    /// Useful for logging, saving checkpoints, or checking stopping conditions.
    /// </summary>
    void OnGenerationEnd(TrainingContext context) { }

    /// <summary>
    /// Called once after the training loop finishes (or is aborted).
    /// Useful for cleanup.
    /// </summary>
    void OnSessionEnd(TrainingContext context) { }
}