namespace AeroNeuro.Core.Training.Decorators.TaskMaster;

public interface IDistortionSource<T>
{
    /// <summary>
    /// Takes the real observation and returns the distorted one.
    /// </summary>
    T[] Distort(T[] observation);

    /// <summary>
    /// Called when the environment resets. 
    /// Useful for swapping the TaskMaster or resetting LSTM memory.
    /// </summary>
    void Reset();
}
