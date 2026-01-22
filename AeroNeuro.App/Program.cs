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

var agentProvider = new GenomeAgentProvider(environment, mutationStrategy, outputExtractor, programExecutor, 128, networkSize: 32, memorySize: 64);
var fitnessEvaluator = new TrainingFitnessEvaluator<byte>(environment, 100, 100);
var populationSelector = new TopFractionPopulationSelector<byte>(0.1f);
var agentPersistence = new GenomeAgentPersistence(mutationStrategy, outputExtractor, programExecutor);
var statsDisplayer = new StatsDisplayer(Console.WriteLine);

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
    .WithConditionalAction(stats => stats.Generation % 50 == 0, (IEnvironment<byte> env) => { if (env is EscapeRoomEnvironment e) e.PrintBoard(); })
    .WithStatsDisplayer(statsDisplayer)
    .WithPopulationSize(50);

var trainingSession = builder.Build();

Console.WriteLine("Starting training...");
trainingSession.Run(500);
Console.WriteLine("Training finished.");