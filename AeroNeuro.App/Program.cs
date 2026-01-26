using AeroNeuro.Core;
using AeroNeuro.Core.Agents.Abstractions;
using AeroNeuro.Core.Agents.Execution;
using AeroNeuro.Core.Agents.Implementations.GenomeAgent;
using AeroNeuro.Core.Agents.Mutation;
using AeroNeuro.Core.Common;
using AeroNeuro.Core.Environments.Concrete;
using AeroNeuro.Core.Training;
using AeroNeuro.Core.Training.Evaluation;
using AeroNeuro.Core.Training.Selection;
using AeroNeuro.Core.Training.Session;
using System.Globalization;
using System.Linq;

// Setup
const int populationSize = 32;
const string trainingDataPath = "training_data.txt";
const string agentSavePath = "best_agent.json";

// 1. Environment
var environment = new ByteTrainingEnvironment(trainingDataPath, contextWindowSize: 64, stepsPerEpisode: 200);

// 2. Core agent components
var evolutionStatsProvider = new EvolutionStatsProvider();
var memoryOutputExtractor = new MemoryOutputExtractor(environment.ActionSize);
var programExecutor = new BasicProgramExecutor();
var mutationStrategy = new StallDetectorMutationStrategy(evolutionStatsProvider, generationDelay: 10, fitnessThreshold: 0.01f);

// 3. Agent Provider
var agentTopology = new AgentTopology(environment.ObservationSize, environment.ActionSize);
var agentProvider = new GenomeAgentProvider(
    agentTopology,
    mutationStrategy,
    memoryOutputExtractor,
    programExecutor,
    maxNetworkSize: 512,
    networkSize: 64,
    memorySize: 256
);

// 4. Initial Population
var populationProvider = new PopulationProvider<byte>
{
    PopulationSize = populationSize
};
for (int i = 0; i < populationSize; i++)
{
    populationProvider.Population.Add(agentProvider.CreateRandomAgent(minNodes: 32, maxNodes: 64));
}


// 5. Training components
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, episodes: 3, timeWeight: 0, maxSteps: 200);
var populationSelector = new OutlierPopulationSelector<byte>(minAgents: 8, maxAgents: 16);

// 6. Hooks
var consoleLogHook = new GenerationEndHook<byte>(context =>
{
    var stats = context.Stats;
    Console.WriteLine(
        $"Gen: {stats.Generation.ToString(CultureInfo.InvariantCulture)} | " +
        $"Best Fitness: {stats.BestFitness.ToString("F3", CultureInfo.InvariantCulture)} | " +
        $"Avg Fitness: {stats.AverageFitness.ToString("F3", CultureInfo.InvariantCulture)} | " +
        $"Worst Fitness: {stats.WorstFitness.ToString("F3", CultureInfo.InvariantCulture)} | " +
        $"Duration: {stats.GenerationDuration.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture)}ms"
    );
});

var agentPersistence = new GenomeAgentPersistence(mutationStrategy, memoryOutputExtractor, programExecutor);
var saveBestAgentHook = new GenerationEndHook<byte>(context =>
{
    if (context.Stats.Generation % 20 != 0) return;

    var population = populationProvider.Population;
    if (population?.Any() != true) return;

    var bestAgent = population
        .AsParallel()
        .Select(agent => new { Agent = agent, Fitness = fitnessEvaluator.Evaluate(agent) })
        .OrderByDescending(x => x.Fitness)
        .FirstOrDefault()?
        .Agent;

    if (bestAgent != null)
    {
        agentPersistence.SaveAgent(agentSavePath, bestAgent);
    }
});


// 7. Builder
var builder = new AeroNeuroBuilder<byte>()
    .WithAgentProvider(agentProvider)
    .WithPopulationSelector(populationSelector)
    .WithFitnessEvaluator(fitnessEvaluator)
    .WithPopulationSize(populationSize)
    .WithStatsProvider(evolutionStatsProvider)
    .WithPopulationProvider(populationProvider)
    .WithHook(consoleLogHook)
    .WithHook(saveBestAgentHook);

// 8. Build and Run
var session = builder.Build();
session.Run(generations: 1000);