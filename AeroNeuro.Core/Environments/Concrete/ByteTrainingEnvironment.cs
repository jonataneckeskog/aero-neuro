using AeroNeuro.Common;

namespace AeroNeuro.Core.Environments;

public class ByteTrainingEnvironment : IEnvironment<byte>
{
    private readonly byte[] _data;
    private readonly int _contextWindowSize;
    private readonly int _maxStepsPerEpisode;

    private int _currentPosition;
    private int _stepsTaken;

    /// <inheritdoc />
    public int ObservationSize => _contextWindowSize;

    /// <inheritdoc />
    public int ActionSize => 1;

    /// <inheritdoc />
    // Change >= to >
    public bool IsDone => _stepsTaken >= _maxStepsPerEpisode ||
                        _currentPosition + _contextWindowSize + 1 > _data.Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteTrainingEnvironment"/> class.
    /// </summary>
    /// <param name="filePath">Path to the source text file.</param>
    /// <param name="contextWindowSize">The size of the sliding observation window.</param>
    /// <param name="stepsPerEpisode">Maximum steps allowed per episode.</param>
    public ByteTrainingEnvironment(string filePath, int contextWindowSize = 64, int stepsPerEpisode = 1000)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Training file not found at {filePath}");

        _data = File.ReadAllBytes(filePath);
        _contextWindowSize = contextWindowSize;
        _maxStepsPerEpisode = stepsPerEpisode;

        if (_data.Length <= _contextWindowSize + 1)
            throw new ArgumentException("File is too small for the requested window size.");

        Reset();
    }

    /// <inheritdoc />
    public byte[] GetObservation()
    {
        if (IsDone) return new byte[_contextWindowSize];

        byte[] observation = new byte[_contextWindowSize];
        Array.Copy(_data, _currentPosition, observation, 0, _contextWindowSize);
        return observation;
    }

    /// <inheritdoc />
    public float Step(byte[] actions)
    {
        if (IsDone) return 0f;

        byte predictedByte = actions[0];

        int targetIndex = _currentPosition + _contextWindowSize;
        byte actualByte = _data[targetIndex];

        float reward = (predictedByte == actualByte) ? 1.0f : -0.1f;

        _currentPosition++;
        _stepsTaken++;

        return reward;
    }

    /// <inheritdoc />
    public void Reset()
    {
        _stepsTaken = 0;

        int safeBuffer = _data.Length - _contextWindowSize - _maxStepsPerEpisode;

        if (safeBuffer > 0)
        {
            _currentPosition = ThreadSafeRandom.Instance.Next(0, safeBuffer);
        }
        else
        {
            int maxStart = _data.Length - _contextWindowSize - 1;
            _currentPosition = ThreadSafeRandom.Instance.Next(0, maxStart > 0 ? maxStart : 0);
        }
    }
}
