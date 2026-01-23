using AeroNeuro.Core;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Agents.GenomeAgent;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;
using System.Globalization;
using PolylineSimplifier;

const string AgentSavePath = "best_agent.json";
const string DataOutputPath = "byte_training_output.txt";

var environment = new ByteTrainingEnvironment("training_data.txt", contextWindowSize: 16, stepsPerEpisode: 20);
var mutationStrategy = new BasicMutationStrategy();
var outputExtractor = new OutputExtractor(environment.ActionSize);
var programExecutor = new BasicProgramExecutor();

var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor, 128, networkSize: 128, memorySize: 256);
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, 10, 5000);
var populationSelector = new TopFractionPopulationSelector<byte>(0.2f);
var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);
var statsDisplayer = new StatsDisplayer(stats =>
{
    Console.WriteLine($"Generation: {stats.Generation}, Best Fitness: {stats.BestFitness}, Average Fitness: {stats.AverageFitness:F2}");

    string csvLine = $"{stats.Generation},{stats.BestFitness},{stats.AverageFitness}{Environment.NewLine}";
    File.AppendAllText("training_stats.csv", csvLine);
});

var displayerHook = new EnvironmentDisplayerHook(DataOutputPath);

var builder = new AeroNeuroBuilder<byte>()
    .WithEnvironment(environment)
    .WithAgentProvider(agentProvider)
    .WithFitnessEvaluator(fitnessEvaluator)
    .WithPopulationSelector(populationSelector)
    .WithConditionalAction(stats => stats.Generation % 50 == 0, (IEvolutionTrainer<byte> trainer) =>
    {
        var bestAgent = trainer.GetBestAgents(1).FirstOrDefault();
        if (bestAgent != null)
        {
            agentPersistence.SaveAgent(AgentSavePath, bestAgent);
        }
    })
    .WithConditionalAction(stats => (stats.Generation + 1) % 100 == 0, displayerHook.DisplayEnvironment)
    .WithStatsDisplayer(statsDisplayer)
    .WithPopulationSize(20);

var trainingSession = builder.Build();

Console.WriteLine("Starting training...");
trainingSession.Run(5000);
Console.WriteLine("Training finished.");


Console.WriteLine("Optimizing CSV for rendering...");

var lines = File.ReadAllLines("training_stats.csv").Skip(1).ToArray(); // Skip header
var dataPoints = new List<StatsPoint>();

foreach (var line in lines)
{
    var parts = line.Split(',');
    if (parts.Length >= 2)
    {
        // Parse data
        float gen = float.Parse(parts[0], CultureInfo.InvariantCulture);
        float bestFit = float.Parse(parts[1], CultureInfo.InvariantCulture);

        // Store the parsed values AND the original line string
        dataPoints.Add(new StatsPoint(gen, bestFit, line));
    }
}

// SIMPLIFY
// Using the generic signature from the README. 
// We pass our list of 'StatsPoint' directly.
var simplifiedPoints = RamerDouglasPeucker2D.Simplify(
    dataPoints,
    epsilon: 4f,            // Tolerance: higher = fewer points
    getX: p => p.Generation,  // Lambda: how to get X
    getY: p => p.BestFitness  // Lambda: how to get Y
);

// WRITE OUTPUT
var outputLines = new List<string> { "Generation,BestFitness,AverageFitness" };

// Because the library returns a List<StatsPoint>, we can just grab the .OriginalLine property directly
outputLines.AddRange(simplifiedPoints.Select(p => p.OriginalLine));

File.WriteAllLines("training_stats_optimized.csv", outputLines);

Console.WriteLine($"Optimization Complete.");
Console.WriteLine($"Original points: {dataPoints.Count}");
Console.WriteLine($"Optimized points: {simplifiedPoints.Count}");
Console.WriteLine($"Saved to: training_stats_optimized.csv");

// ---------------------------------------------------------
// HELPER RECORD
// ---------------------------------------------------------
// A simple container to hold the parsed numeric data for the algorithm 
// and the original string for the file output.
public record StatsPoint(float Generation, float BestFitness, string OriginalLine);

/// <summary>
/// Custom environment displayer that logs environment state to a file.
/// </summary>
public class EnvironmentDisplayerHook : IEnvironmentDisplayer<byte>
{
    private readonly string _filePath;

    public EnvironmentDisplayerHook(string filePath)
    {
        _filePath = filePath;
    }

    public void DisplayEnvironment(IEnvironment<byte> environment)
    {
        // 1. Reset to move the cursor to a fresh chunk of data
        environment.Reset();

        // 2. Get the raw bytes
        var observation = environment.GetObservation();

        // 3. Convert bytes to String
        // Using UTF8 is standard for .txt files. 
        string textSnippet = System.Text.Encoding.UTF8.GetString(observation);

        // 4. Sanitize for the log file (replace newlines with spaces so it stays on one line)
        string printable = textSnippet.Replace("\r", "").Replace("\n", "[\\n]");

        var logEntry = $"[Gen Observation] \"{printable}\"{Environment.NewLine}";

        File.AppendAllText(_filePath, logEntry);

        // Optional: Also print to console so you can see it's working
        Console.WriteLine($"Current Data Sample: {printable}");
    }
}
