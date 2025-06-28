using System;

namespace ASL.CodeEngineering.AI.OfflineLearning;

/// <summary>
/// Minimal linear model used for offline experiments.
/// </summary>
public class OfflineModel
{
    public Tensor Weights { get; }

    public OfflineModel(int inputSize)
    {
        Weights = Tensor.Random(1, inputSize);
    }

    public double Predict(double[] input)
    {
        if (input.Length != Weights.Cols)
            throw new ArgumentException("Input size mismatch");
        return Weights.Dot(input)[0];
    }
}
