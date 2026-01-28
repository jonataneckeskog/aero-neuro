using AeroNeuro.Core.Environments.Abstractions;

namespace AeroNeuro.Core.Training.Decorators.TaskMaster;

public class DistortionDecorator<T> : IEnvironment<T>
{
    private readonly IEnvironment<T> _environment;
    private readonly IDistortionSource<T> _distortionSource;

    public DistortionDecorator(IEnvironment<T> environment, IDistortionSource<T> distortionSource)
    {
        _environment = environment;
        _distortionSource = distortionSource;
    }

    // Pass-through properties
    public int ObservationSize => _environment.ObservationSize;
    public int ActionSize => _environment.ActionSize;
    public bool IsDone => _environment.IsDone;

    public void Act(T[] actions) => _environment.Act(actions);
    public float Step(T[] actions) => _environment.Step(actions);

    public T[] GetObservation()
    {
        // 1. Get Reality
        var obs = _environment.GetObservation();

        // 2. Apply Distortion Strategy
        return _distortionSource.Distort(obs);
    }

    public void Reset()
    {
        _environment.Reset();

        // 3. Signal the source that a new episode began
        _distortionSource.Reset();
    }
}
