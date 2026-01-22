using AeroNeuro.Core;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Agents.GenomeAgent;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;

const string AgentSavePath = "best_agent.json";

var environment = new Grid2DEnvironment();
var mutationStrategy = new BasicMutationStrategy();
var outputExtractor = new OutputExtractor(environment.ActionSize);
var programExecutor = new BasicProgramExecutor();

var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor);
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, 100);
var populationSelector = new TopFractionPopulationSelector<byte>(0.2f);
var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);
var statsDisplayer = new StatsDisplayer();

var builder = new AeroNeuroBuilder<byte>()
    .WithEnvironment(environment)
    .WithAgentProvider(agentProvider)
    .WithFitnessEvaluator(fitnessEvaluator)
    .WithPopulationSelector(populationSelector)
    .WithAgentPersistence(agentPersistence)
    .WithStatsDisplayer(statsDisplayer)
    .WithPopulationSize(1000)
    .WithMaxStepsPerEpisode(100);

var trainingSession = builder.Build();

Console.WriteLine("Starting training...");
trainingSession.Run(100);
Console.WriteLine("Training finished.");