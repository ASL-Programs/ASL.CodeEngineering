using System;
using System.IO;

namespace ASL.CodeEngineering.AI.OfflineLearning;

/// <summary>
/// Stores copies of trained models under data/models for versioning.
/// </summary>
public static class ModelVersionManager
{
    public static void SaveVersion(string projectRoot, string modelPath)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                            Path.Combine(projectRoot, "data");
        string modelsDir = Path.Combine(baseData, "models");
        Directory.CreateDirectory(modelsDir);
        string stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string destDir = Path.Combine(modelsDir, stamp);
        Directory.CreateDirectory(destDir);
        File.Copy(modelPath, Path.Combine(destDir, Path.GetFileName(modelPath)), true);
    }
}
