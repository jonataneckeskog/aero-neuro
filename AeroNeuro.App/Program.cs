using AeroNeuro.Runner;

// 1. Initialize the builder with the mandatory data path
var builder = new TrainingRunnerBuilder("training_data.txt");

// 2. Configure the hyperparameters fluently
// You only need to call the methods for values you want to change from the defaults.
builder
    .WithPaths(
        output: "session_output.txt",
        agentSave: "pilot_v1.json",
        csv: "v1_metrics.csv"
    )
    .WithAgentDimensions(
        input: 1024,
        hidden: 512,
        output: 1024
    )
    .WithEvolutionSettings(
        popSize: 64,
        mutationThreshold: 2048,
        minMutation: 512
    )
    .WithIntervals(
        save: 100,
        display: 200,
        tolerance: 2.5f // Higher tolerance = more aggressive CSV compression
    );

// 3. Build the runner instance
TrainingRunner runner = builder.Build();

// 4. Start the execution
Console.WriteLine("--- AeroNeuro Training Session Initialized ---");
runner.Run(generations: 1000);