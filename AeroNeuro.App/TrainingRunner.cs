using AeroNeuro.Core;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Agents.GenomeAgent;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;
using System.Globalization;
using PolylineSimplifier;

namespace AeroNeuro.Runner;

public class TrainingRunner
{
    // Paths
    private readonly string _trainingDataPath;
    private readonly string _outputPath;
    private readonly string _agentSavePath;
    private readonly string _csvPath;

    // Hyperparameters
    private readonly int _observationSize;
    private readonly int _actionSize;
    private readonly int _mutationThreshold;
    private readonly int _minMutation;
    private readonly int _inputSize;
    private readonly int _hiddenSize;
    private readonly int _outputSize;
    private readonly int _maxExecutionSteps;

    // Evolution & Selection
    private readonly float _outlierRatio;
    private readonly float _outlierThreshold;
    private readonly int _minPopulation;
    private readonly int _maxPopulation;
    private readonly int _populationSize;

    // Intervals & Optimization
    private readonly int _saveInterval;
    private readonly int _displayInterval;
    private readonly float _simplificationTolerance;

    public TrainingRunner(
        string trainingDataPath, string outputPath, string agentSavePath, string csvPath,
        int observationSize, int actionSize, int mutationThreshold, int minMutation,
        int inputSize, int hiddenSize, int outputSize, int maxExecutionSteps,
        float outlierRatio, float outlierThreshold, int minPopulation, int maxPopulation,
        int populationSize, int saveInterval, int displayInterval, float simplificationTolerance)
    {
        _trainingDataPath = trainingDataPath;
        _outputPath = outputPath;
        _agentSavePath = agentSavePath;
        _csvPath = csvPath;
        _observationSize = observationSize;
        _actionSize = actionSize;
        _mutationThreshold = mutationThreshold;
        _minMutation = minMutation;
        _inputSize = inputSize;
        _hiddenSize = hiddenSize;
        _outputSize = outputSize;
        _maxExecutionSteps = maxExecutionSteps;
        _outlierRatio = outlierRatio;
        _outlierThreshold = outlierThreshold;
        _minPopulation = minPopulation;
        _maxPopulation = maxPopulation;
        _populationSize = populationSize;
        _saveInterval = saveInterval;
        _displayInterval = displayInterval;
        _simplificationTolerance = simplificationTolerance;
    }

    public void Run(int generations)
    {
        var environment = new ByteTrainingEnvironment(_trainingDataPath, _observationSize, _actionSize);
        var trainingStateProvider = new TrainingStateProvider();
        var mutationStrategy = new StallDetectorMutationStrategy(trainingStateProvider, _mutationThreshold, _minMutation);
        var outputExtractor = new OutputExtractor(environment.ActionSize);
        var programExecutor = new BasicProgramExecutor();
        var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);

        var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor, _inputSize, _hiddenSize, _outputSize);
        var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, _observationSize, _inputSize, _maxExecutionSteps);
        var populationSelector = new OutlierPopulationSelector<byte>(_outlierRatio, _outlierThreshold, _minPopulation, _maxPopulation);

        var displayerHook = new EnvironmentDisplayerHook(_outputPath);

        var session = new AeroNeuroBuilder<byte>()
            .WithEnvironment(environment)
            .WithAgentProvider(agentProvider)
            .WithFitnessEvaluator(fitnessEvaluator)
            .WithPopulationSelector(populationSelector)
            .WithStatsDisplayer(CreateStatsDisplayer(trainingStateProvider))
            .WithConditionalAction(s => s.Generation % _saveInterval == 0, (IEvolutionTrainer<byte> t) => SaveBestAgent(t, agentPersistence))
            .WithConditionalAction(s => (s.Generation + 1) % _displayInterval == 0, displayerHook.DisplayEnvironment)
            .WithPopulationSize(_populationSize)
            .Build();

        Console.WriteLine($"Starting training for {generations} generations...");
        session.Run(generations);

        OptimizeCsvOutput();
    }

    private IStatsDisplayer CreateStatsDisplayer(TrainingStateProvider stateProvider)
    {
        return new StatsDisplayer(stats =>
        {
            stateProvider.UpdateStats(stats);
            Console.WriteLine($"Gen: {stats.Generation} | Best: {stats.BestFitness} | Avg: {stats.AverageFitness:F2}");
            File.AppendAllText(_csvPath, $"{stats.Generation},{stats.BestFitness},{stats.AverageFitness}{Environment.NewLine}");
        });
    }

    private void SaveBestAgent(IEvolutionTrainer<byte> trainer, GenomeAgentPersistence persistence)
    {
        var bestAgent = trainer.GetBestAgents(1).FirstOrDefault();
        if (bestAgent != null) persistence.SaveAgent(_agentSavePath, bestAgent);
    }

    private void OptimizeCsvOutput()
    {
        if (!File.Exists(_csvPath)) return;

        var lines = File.ReadAllLines(_csvPath).Skip(1).ToArray();
        var dataPoints = lines.Select(line =>
        {
            var parts = line.Split(',');
            return new StatsPoint(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), line);
        }).ToList();

        var simplified = RamerDouglasPeucker2D.Simplify(dataPoints, _simplificationTolerance, p => p.Generation, p => p.BestFitness);

        var output = new List<string> { "Generation,BestFitness,AverageFitness" };
        output.AddRange(simplified.Select(p => p.OriginalLine));
        File.WriteAllLines("training_stats_optimized.csv", output);
    }
}

// Keep your helper records and displayers here or in separate files
public record StatsPoint(float Generation, float BestFitness, string OriginalLine);

public class EnvironmentDisplayerHook : IEnvironmentDisplayer<byte>
{
    private readonly string _filePath;
    public EnvironmentDisplayerHook(string filePath) => _filePath = filePath;

    public void DisplayEnvironment(IEnvironment<byte> environment)
    {
        environment.Reset();
        var printable = System.Text.Encoding.UTF8.GetString(environment.GetObservation())
            .Replace("\r", "").Replace("\n", "[\\n]");

        File.AppendAllText(_filePath, $"[Gen Observation] \"{printable}\"{Environment.NewLine}");
        Console.WriteLine($"Current Data Sample: {printable}");
    }
}