namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// A simple hook that executes a specific action at the end of every generation.
/// </summary>
public class GenerationEndHook<T> : ITrainingSessionHook<T>
{
    private readonly Action<TrainingContext<T>> _onGenerationEndAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationEndHook{T}"/> class.
    /// </summary>
    /// <param name="onGenerationEndAction">The action to execute when a generation finishes.</param>
    public GenerationEndHook(Action<TrainingContext<T>> onGenerationEndAction)
    {
        _onGenerationEndAction = onGenerationEndAction
            ?? throw new ArgumentNullException(nameof(onGenerationEndAction));
    }

    /// <inheritdoc/>
    public void OnGenerationEnd(TrainingContext<T> context)
    {
        _onGenerationEndAction(context);
    }
}
