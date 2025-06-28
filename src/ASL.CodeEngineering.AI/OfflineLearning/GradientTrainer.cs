using System;
using System.Collections.Generic;

namespace ASL.CodeEngineering.AI.OfflineLearning;

/// <summary>
/// Very small gradient descent trainer for <see cref="OfflineModel"/>.
/// </summary>
public static class GradientTrainer
{
    public static void Train(OfflineModel model, IList<double[]> inputs, IList<double> targets, double learningRate, int epochs)
    {
        if (inputs.Count != targets.Count)
            throw new ArgumentException("Input/target size mismatch");

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                var x = inputs[i];
                double y = targets[i];
                double pred = model.Predict(x);
                double error = pred - y;
                for (int j = 0; j < x.Length; j++)
                {
                    double grad = error * x[j];
                    model.Weights[0, j] -= learningRate * grad;
                }
            }
        }
    }
}
