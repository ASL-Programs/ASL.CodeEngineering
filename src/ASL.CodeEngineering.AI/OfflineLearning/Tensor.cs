using System;

namespace ASL.CodeEngineering.AI.OfflineLearning;

/// <summary>
/// Simple 2D tensor for offline learning operations.
/// </summary>
public class Tensor
{
    public int Rows { get; }
    public int Cols { get; }
    private readonly double[] _data;

    public Tensor(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _data = new double[rows * cols];
    }

    public double this[int row, int col]
    {
        get => _data[row * Cols + col];
        set => _data[row * Cols + col] = value;
    }

    public static Tensor Random(int rows, int cols, Random? random = null)
    {
        var t = new Tensor(rows, cols);
        random ??= Random.Shared;
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            t[i, j] = random.NextDouble() - 0.5;
        return t;
    }

    public double[] Dot(double[] vector)
    {
        if (vector.Length != Cols)
            throw new ArgumentException("Vector length must match tensor columns");
        var result = new double[Rows];
        for (int i = 0; i < Rows; i++)
        {
            double sum = 0;
            for (int j = 0; j < Cols; j++)
                sum += this[i, j] * vector[j];
            result[i] = sum;
        }
        return result;
    }
}
