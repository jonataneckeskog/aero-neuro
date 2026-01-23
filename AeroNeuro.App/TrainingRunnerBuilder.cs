namespace AeroNeuro.Runner;

public class TrainingRunnerBuilder
{
    private string _trainingDataPath;
    private string _outputPath = "byte_training_output.txt";
    private string _agentSavePath = "best_agent.json";
    private string _csvPath = "training_stats.csv";

    private int _obsSize = 64;
    private int _actSize = 16;
    private int _mutationThreshold = 1028;
    private int _minMutation = 256;
    private int _inputSize = 1028;
    private int _hiddenSize = 512;
    private int _outputSize = 1028;
    private int _maxSteps = 1000;

    private float _outlierRatio = 0.05f;
    private float _outlierThreshold = 1.5f;
    private int _minPop = 4;
    private int _maxPop = 8;
    private int _popSize = 32;

    private int _saveInterval = 50;
    private int _displayInterval = 100;
    private float _tolerance = 4f;

    public TrainingRunnerBuilder(string trainingDataPath) => _trainingDataPath = trainingDataPath;

    public TrainingRunnerBuilder WithPaths(string output, string agentSave, string csv)
    {
        _outputPath = output; _agentSavePath = agentSave; _csvPath = csv;
        return this;
    }

    public TrainingRunnerBuilder WithAgentDimensions(int input, int hidden, int output)
    {
        _inputSize = input; _hiddenSize = hidden; _outputSize = output;
        return this;
    }

    public TrainingRunnerBuilder WithEvolutionSettings(int popSize, int mutationThreshold, int minMutation)
    {
        _popSize = popSize; _mutationThreshold = mutationThreshold; _minMutation = minMutation;
        return this;
    }

    public TrainingRunnerBuilder WithSelection(float ratio, float threshold, int min, int max)
    {
        _outlierRatio = ratio; _outlierThreshold = threshold; _minPop = min; _maxPop = max;
        return this;
    }

    public TrainingRunnerBuilder WithIntervals(int save, int display, float tolerance = 4f)
    {
        _saveInterval = save; _displayInterval = display; _tolerance = tolerance;
        return this;
    }

    public TrainingRunner Build()
    {
        return new TrainingRunner(
            _trainingDataPath, _outputPath, _agentSavePath, _csvPath,
            _obsSize, _actSize, _mutationThreshold, _minMutation,
            _inputSize, _hiddenSize, _outputSize, _maxSteps,
            _outlierRatio, _outlierThreshold, _minPop, _maxPop,
            _popSize, _saveInterval, _displayInterval, _tolerance
        );
    }
}