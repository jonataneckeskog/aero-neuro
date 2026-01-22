namespace AeroNeuro.Core.Environments;

/// <summary>
/// A 5x5 Grid World where the agent must pick up a Key before exiting through a Door.
/// T is byte.
/// </summary>
public class EscapeRoomEnvironment : IEnvironment<byte>
{
    private const int GridSize = 5;
    private const int MaxSteps = 50;

    // Coordinates
    private int _agentX, _agentY;
    private int _keyX, _keyY;
    private int _doorX, _doorY;

    // State
    private bool _hasKey;
    private int _currentStep;
    private readonly Random _random = new Random();

    // Observation Layout (Size 7):
    // [0] Agent X
    // [1] Agent Y
    // [2] Key X
    // [3] Key Y
    // [4] Door X
    // [5] Door Y
    // [6] HasKey (0 or 1)
    public int ObservationSize => 7;

    // Action Layout (Size 1):
    // [0] Direction (0: Up, 1: Down, 2: Left, 3: Right)
    public int ActionSize => 1;

    public bool IsDone { get; private set; }

    public EscapeRoomEnvironment()
    {
        Reset();
    }

    public byte[] GetObservation()
    {
        return new byte[]
        {
                (byte)_agentX,
                (byte)_agentY,
                (byte)_keyX,
                (byte)_keyY,
                (byte)_doorX,
                (byte)_doorY,
                (byte)(_hasKey ? 1 : 0)
        };
    }

    public float Step(byte[] actions)
    {
        if (IsDone) return 0;
        if (actions == null || actions.Length == 0) return 0;

        _currentStep++;
        float reward = -0.1f; // Small time penalty to encourage efficiency

        // 1. Process Movement
        int move = actions[0]; // We only use the first byte
        int newX = _agentX;
        int newY = _agentY;

        switch (move)
        {
            case 0: newY--; break; // Up
            case 1: newY++; break; // Down
            case 2: newX--; break; // Left
            case 3: newX++; break; // Right
        }

        // 2. Bound Checks (Walls)
        // If move is invalid, agent stays in place
        if (newX >= 0 && newX < GridSize && newY >= 0 && newY < GridSize)
        {
            _agentX = newX;
            _agentY = newY;
        }

        // 3. Logic: Check Key Pickup
        // If we are at the key location and don't have it yet
        if (!_hasKey && _agentX == _keyX && _agentY == _keyY)
        {
            _hasKey = true;
            reward += 10.0f; // Reward for finding the necessary tool
                             // Teleport key off the map or keep it there (conceptually)
                             // In observation, the coordinates remain, but HasKey flag changes.
        }

        // 4. Logic: Check Door Exit
        if (_agentX == _doorX && _agentY == _doorY)
        {
            if (_hasKey)
            {
                reward += 100.0f; // Big reward for solving the puzzle
                IsDone = true;
                return reward;
            }
            else
            {
                // Optional: Penalty for hitting the door without the key?
                // For now, just standard time penalty.
            }
        }

        // 5. Check Termination (Timeout)
        if (_currentStep >= MaxSteps)
        {
            IsDone = true;
            reward -= 5.0f; // Failure penalty
        }

        return reward;
    }

    public void Reset()
    {
        _currentStep = 0;
        _hasKey = false;
        IsDone = false;

        // Generate random positions, ensuring they don't overlap at start
        // to prevent instant wins or confusion.
        _agentX = _random.Next(0, GridSize);
        _agentY = _random.Next(0, GridSize);

        do
        {
            _keyX = _random.Next(0, GridSize);
            _keyY = _random.Next(0, GridSize);
        } while (_keyX == _agentX && _keyY == _agentY);

        do
        {
            _doorX = _random.Next(0, GridSize);
            _doorY = _random.Next(0, GridSize);
        } while ((_doorX == _agentX && _doorY == _agentY) ||
                 (_doorX == _keyX && _doorY == _keyY));
    }

    /// <summary>
    /// Helper to visualize the state in Console
    /// </summary>
    public void PrintBoard()
    {
        Console.WriteLine($"--- Step: {_currentStep} | Has Key: {_hasKey} ---");
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                if (x == _agentX && y == _agentY) Console.Write("A ");
                else if (x == _keyX && y == _keyY && !_hasKey) Console.Write("K ");
                else if (x == _doorX && y == _doorY) Console.Write("D ");
                else Console.Write(". ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
