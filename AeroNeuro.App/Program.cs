using AeroNeuro.Core;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Agents.GenomeAgent;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;
using System.Globalization;
using PolylineSimplifier;

const string AgentSavePath = "best_agent.json";

var environment = new EscapeRoomEnvironment();
var mutationStrategy = new BasicMutationStrategy();
var outputExtractor = new OutputExtractor(environment.ActionSize);
var programExecutor = new BasicProgramExecutor();

var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor, 128, networkSize: 32, memorySize: 64);
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, 50, 1000);
var populationSelector = new TopFractionPopulationSelector<byte>(0.1f);
var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);
var statsDisplayer = new StatsDisplayer(stats =>
{
    Console.WriteLine($"Generation: {stats.Generation}, Best Fitness: {stats.BestFitness}, Average Fitness: {stats.AverageFitness:F2}");

    string csvLine = $"{stats.Generation},{stats.BestFitness},{stats.AverageFitness}{Environment.NewLine}";
    File.AppendAllText("training_stats.csv", csvLine);
});

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
    .WithStatsDisplayer(statsDisplayer)
    .WithPopulationSize(50);

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
    epsilon: 2.2f,            // Tolerance: higher = fewer points
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