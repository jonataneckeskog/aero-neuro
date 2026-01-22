using AeroNeuro.Core;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Agents.GenomeAgent;
using AeroNeuro.Core.Environments;
using AeroNeuro.Core.Training;

const string AgentSavePath = "best_agent.json";

var environment = new EscapeRoomEnvironment();
var mutationStrategy = new BasicMutationStrategy();
var outputExtractor = new OutputExtractor(environment.ActionSize);
var programExecutor = new BasicProgramExecutor();

var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor, networkSize: 128, memorySize: 64);
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, 10000, 10);
var populationSelector = new TopFractionPopulationSelector<byte>(0.1f);
var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);
var statsDisplayer = new StatsDisplayer();

var builder = new AeroNeuroBuilder<byte>()
    .WithEnvironment(environment)
    .WithAgentProvider(agentProvider)
    .WithFitnessEvaluator(fitnessEvaluator)
    .WithPopulationSelector(populationSelector)
    .WithAgentPersistence(agentPersistence)
    .WithStatsDisplayer(statsDisplayer)
    .WithPopulationSize(50)
    .WithMaxStepsPerEpisode(100);

var trainingSession = builder.Build();

Console.WriteLine("Starting training...");
trainingSession.Run(10000);
Console.WriteLine("Training finished.");