using System.IO;
using System.Text.Json;

namespace ASL.CodeEngineering.AI.OfflineLearning;

/// <summary>
/// Saves and loads simple models in .pt or .onnx format.
/// The implementation stores weights as JSON for demonstration purposes.
/// </summary>
public static class ModelLoader
{
    public static void SavePt(OfflineModel model, string path)
    {
        Save(model, path);
    }

    public static void SaveOnnx(OfflineModel model, string path)
    {
        Save(model, path);
    }

    public static OfflineModel LoadPt(string path)
    {
        return Load(path);
    }

    public static OfflineModel LoadOnnx(string path)
    {
        return Load(path);
    }

    private static void Save(OfflineModel model, string path)
    {
        var weights = new double[model.Weights.Cols];
        for (int i = 0; i < weights.Length; i++)
            weights[i] = model.Weights[0, i];
        string json = JsonSerializer.Serialize(weights);
        File.WriteAllText(path, json);
    }

    private static OfflineModel Load(string path)
    {
        if (!File.Exists(path))
            return InitializeFromDataset();

        string json = File.ReadAllText(path);
        var weights = JsonSerializer.Deserialize<double[]>(json)!;
        var model = new OfflineModel(weights.Length);
        for (int i = 0; i < weights.Length; i++)
            model.Weights[0, i] = weights[i];
        return model;
    }

    private static OfflineModel InitializeFromDataset()
    {
        string root = AppContext.BaseDirectory;
        string kb = Environment.GetEnvironmentVariable("KB_DIR") ??
                    Path.Combine(root, "knowledge_base");
        string dataPath = Path.Combine(kb, "offline_training", "data.csv");
        if (!File.Exists(dataPath))
            return new OfflineModel(1);

        try
        {
            var lines = File.ReadAllLines(dataPath);
            if (lines.Length <= 1)
                return new OfflineModel(1);
            double sumXY = 0;
            double sumXX = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 2)
                    continue;
                if (double.TryParse(parts[0], out double x) &&
                    double.TryParse(parts[1], out double y))
                {
                    sumXY += x * y;
                    sumXX += x * x;
                }
            }

            var model = new OfflineModel(1);
            if (sumXX > 0)
                model.Weights[0, 0] = sumXY / sumXX;
            else
                model.Weights[0, 0] = 0;
            return model;
        }
        catch
        {
            return new OfflineModel(1);
        }
    }
}
