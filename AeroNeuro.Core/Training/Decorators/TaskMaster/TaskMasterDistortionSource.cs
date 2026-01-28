namespace AeroNeuro.Core.Training.Decorators.TaskMaster;

public class TaskMasterDistortionSource<T> : IDistortionSource<T>
{
    /// <inheritdoc/>
    public T[] Distort(T[] observation)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Reset()
    {
        throw new NotImplementedException();
    }
}