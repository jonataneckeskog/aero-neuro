namespace AeroNeuro.Core.Agents;

public class DoubleNetworkAgent : IAgent
{
    private readonly int _input;
    private readonly int _output;

    private float[] _innerLayer;
    private float[] _outerLayer;

    public DoubleNetworkAgent(int input, int output, int innerCount = 50, int outerCount = 20)
    {
        _input = input;
        _output = output;
        _innerLayer = new float[innerCount];
        _outerLayer = new float[outerCount];
    }

    private DoubleNetworkAgent(int input, int output, float[] inner, float[] outer)
    {
        _input = input;
        _output = output;
        _innerLayer = inner;
        _outerLayer = outer;
    }

    /// <inheritdoc/>
    public float[] Decide(float[] observations)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IAgent Clone()
    {
        return new DoubleNetworkAgent(_input, _output,
                                     (float[])_innerLayer.Clone(),
                                     (float[])_outerLayer.Clone());
    }

    /// <inheritdoc/>
    public void Mutate()
    {
        throw new NotImplementedException();
    }
}