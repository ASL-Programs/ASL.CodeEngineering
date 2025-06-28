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
        string json = File.ReadAllText(path);
        var weights = JsonSerializer.Deserialize<double[]>(json)!;
        var model = new OfflineModel(weights.Length);
        for (int i = 0; i < weights.Length; i++)
            model.Weights[0, i] = weights[i];
        return model;
    }
}
