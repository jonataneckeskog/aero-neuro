using AeroNeuro.Common;

namespace AeroNeuro.Core.Environments;

/// <summary>
/// A simple 2D grid environment where an agent must navigate to the center.
/// By pure accident, this environment actually rewards staying by the center
/// but not actually reaching it, since in that case is just keeps getting max reward.
/// </summary>
public class Grid2DEnvironment : IEnvironment<byte>
{
    private const int GridSize = 8;
    private int _playerX;
    private int _playerY;

    // Target center coordinates (between indices 3 and 4)
    private const float CenterX = 3.5f;
    private const float CenterY = 3.5f;

    private int _steps;
    private const int MaxSteps = 100;
    private readonly float _maxDistance;

    public Grid2DEnvironment()
    {
        _maxDistance = (float)Math.Sqrt(2 * Math.Pow(3.5, 2));
        Reset();
    }

    /// <inheritdoc/>
    public int ObservationSize => 2;

    /// <inheritdoc/>
    public int ActionSize => 2;

    /// <inheritdoc/>
    public bool IsDone => _steps >= MaxSteps ||
                         (_playerX >= 3 && _playerX <= 4 && _playerY >= 3 && _playerY <= 4);

    /// <inheritdoc/>
    public byte[] GetObservation()
    {
        byte[] obs = new byte[ObservationSize];
        obs[0] = (byte)_playerX;
        obs[1] = (byte)_playerY;
        return obs;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _playerX = ThreadSafeRandom.Instance.Next(GridSize);
        _playerY = ThreadSafeRandom.Instance.Next(GridSize);
        _steps = 0;
    }

    /// <inheritdoc/>
    public float Step(byte[] actions)
    {
        if (actions.Length < ActionSize) return 0.0f;

        if (IsDone) return 1.0f;

        _steps++;

        sbyte rawDeltaX = (sbyte)actions[0];
        sbyte rawDeltaY = (sbyte)actions[1];

        _playerX = Math.Clamp(_playerX + Math.Sign(rawDeltaX), 0, GridSize - 1);
        _playerY = Math.Clamp(_playerY + Math.Sign(rawDeltaY), 0, GridSize - 1);

        if (_playerX >= 3 && _playerX <= 4 && _playerY >= 3 && _playerY <= 4)
        {
            return 1.0f;
        }

        float distance = (float)Math.Sqrt(
            Math.Pow(_playerX - CenterX, 2) +
            Math.Pow(_playerY - CenterY, 2));

        return Math.Max(0.0f, 1.0f - (distance / _maxDistance));
    }
}
