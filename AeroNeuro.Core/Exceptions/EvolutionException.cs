namespace AeroNeuro.Core.Exceptions;

public class EvolutionException : Exception
{
    public EvolutionException(string message) : base(message) { }

    public static EvolutionException EmptySelection() =>
        new EvolutionException("The population selector returned no agents. Evolution cannot continue.");
}