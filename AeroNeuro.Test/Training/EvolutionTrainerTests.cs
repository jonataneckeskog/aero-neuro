using Moq;
using AeroNeuro.Core.Agents;
using AeroNeuro.Core.Training;
using AeroNeuro.Test.Mocks;
using AeroNeuro.Core.Exceptions;

namespace AeroNeuro.Test.Training;

public class EvolutionTrainerTests
{
    private readonly Mock<IAgentProvider> _mockAgentProvider;
    private readonly Mock<IFitnessEvaluator> _mockFitnessEvaluator;
    private readonly Mock<IPopulationSelector> _mockPopulationSelector;
    private readonly List<IAgent> _initialPopulation;
    private const int PopulationSize = 10;

    public EvolutionTrainerTests()
    {
        _initialPopulation = MockAgent.CreateList(PopulationSize);
        _mockAgentProvider = MockAgentProvider.Create(_initialPopulation);
        _mockFitnessEvaluator = MockFitnessEvaluator.Create();
        _mockPopulationSelector = MockPopulationSelector.Create();
    }

    private EvolutionTrainer CreateTrainer()
    {
        return new EvolutionTrainer(
            _mockAgentProvider.Object,
            _mockFitnessEvaluator.Object,
            _mockPopulationSelector.Object,
            PopulationSize);
    }

    [Fact]
    public void EvolveGeneration_HappyPath_StatsAreUpdatedCorrectly()
    {
        EvolutionTrainer trainer = CreateTrainer();
        _mockFitnessEvaluator.Setup(e => e.Evaluate(It.IsAny<IAgent>())).Returns(10f);
        _mockPopulationSelector.Setup(s => s.SelectPopulation(It.IsAny<List<(float, IAgent)>>()))
            .Returns((List<(float Fitness, IAgent Agent)> pop) => pop.Select(p => p.Agent).ToList());

        trainer.EvolveGeneration();
        EvolutionStats stats = trainer.GetStats();

        Assert.Equal(1, stats.Generation);
        Assert.Equal(10f, stats.BestFitness);
        Assert.Equal(10f, stats.AverageFitness);
        Assert.Equal(10f, stats.WorstFitness);
        Assert.True(stats.GenerationDuration > TimeSpan.Zero);
    }

    [Fact]
    public void GetBestAgents_AfterOneGeneration_ReturnsCorrectlySortedAgents()
    {
        EvolutionTrainer trainer = CreateTrainer();
        Dictionary<IAgent, float> agentFitness = new Dictionary<IAgent, float>();
        float currentFitness = 100f;
        foreach (IAgent agent in _initialPopulation)
        {
            agentFitness[agent] = currentFitness;
            currentFitness -= 10f;
        }
        _mockFitnessEvaluator.Setup(e => e.Evaluate(It.IsAny<IAgent>()))
            .Returns<IAgent>(agent => agentFitness[agent]);

        trainer.EvolveGeneration();

        List<IAgent> bestAgents = trainer.GetBestAgents(3);

        Assert.Equal(3, bestAgents.Count);
        Assert.Equal(_initialPopulation[0], bestAgents[0]);
        Assert.Equal(_initialPopulation[1], bestAgents[1]);
        Assert.Equal(_initialPopulation[2], bestAgents[2]);
    }

    [Fact]
    public void EvolveGeneration_WithNoElitesSelected_ThrowsException()
    {
        EvolutionTrainer trainer = CreateTrainer();
        _mockFitnessEvaluator.Setup(e => e.Evaluate(It.IsAny<IAgent>())).Returns(1.0f);
        _mockPopulationSelector.Setup(s => s.SelectPopulation(It.IsAny<List<(float, IAgent)>>()))
            .Returns(new List<IAgent>()); // No elites

        Assert.Throws<EvolutionException>(() => trainer.EvolveGeneration());
    }

    [Fact]
    public void Constructor_InitializesPopulationOfCorrectSize()
    {
        EvolutionTrainer trainer = CreateTrainer();

        _mockAgentProvider.Verify(p => p.CreateRandomAgent(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(PopulationSize));
    }

    [Fact]
    public void EvolveGeneration_EvaluatesEveryAgentInPopulation()
    {
        EvolutionTrainer trainer = CreateTrainer();
        _mockPopulationSelector.Setup(s => s.SelectPopulation(It.IsAny<List<(float, IAgent)>>()))
           .Returns(new List<IAgent> { _initialPopulation[0] });

        trainer.EvolveGeneration();

        foreach (IAgent agent in _initialPopulation)
        {
            _mockFitnessEvaluator.Verify(e => e.Evaluate(agent), Times.Once());
        }
    }
}
