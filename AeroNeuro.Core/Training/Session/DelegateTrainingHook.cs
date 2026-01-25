namespace AeroNeuro.Core.Training.Session;

/// <summary>
/// A simple hook that executes a specific action at the end of every generation.
/// </summary>
public class GenerationEndHook : ITrainingSessionHook
{
    private readonly Action<TrainingContext> _onGenerationEndAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationEndHook{T}"/> class.
    /// </summary>
    /// <param name="onGenerationEndAction">The action to execute when a generation finishes.</param>
    public GenerationEndHook(Action<TrainingContext> onGenerationEndAction)
    {
        _onGenerationEndAction = onGenerationEndAction
            ?? throw new ArgumentNullException(nameof(onGenerationEndAction));
    }

    /// <inheritdoc/>
    public void OnGenerationEnd(TrainingContext context)
    {
        _onGenerationEndAction(context);
    }
}
