using System;
using System.Collections.Generic;
using System.IO;
using ASL.CodeEngineering.AI.OfflineLearning;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class OfflineLearningTests : IDisposable
{
    private readonly string _dir;

    public OfflineLearningTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_dir);
    }

    [Fact]
    public void ModelLoader_SaveAndLoadPt_RoundTrips()
    {
        var model = new OfflineModel(2);
        model.Weights[0,0] = 1.5;
        model.Weights[0,1] = -0.4;
        string path = Path.Combine(_dir, "model.pt");

        ModelLoader.SavePt(model, path);
        var loaded = ModelLoader.LoadPt(path);

        Assert.Equal(model.Weights[0,0], loaded.Weights[0,0], 3);
        Assert.Equal(model.Weights[0,1], loaded.Weights[0,1], 3);
    }

    [Fact]
    public void ModelLoader_SaveAndLoadOnnx_RoundTrips()
    {
        var model = new OfflineModel(1);
        model.Weights[0,0] = 0.2;
        string path = Path.Combine(_dir, "model.onnx");

        ModelLoader.SaveOnnx(model, path);
        var loaded = ModelLoader.LoadOnnx(path);

        Assert.Equal(model.Weights[0,0], loaded.Weights[0,0], 3);
    }

    [Fact]
    public void GradientTrainer_UpdatesWeights()
    {
        var model = new OfflineModel(1);
        model.Weights[0,0] = 0;
        var inputs = new List<double[]> { new[] {1.0} };
        var targets = new List<double> { 1.0 };

        GradientTrainer.Train(model, inputs, targets, 0.1, 1);

        Assert.InRange(model.Weights[0,0], 0.0999, 0.1001);
    }

    [Fact]
    public void ModelVersionManager_SaveVersion_CreatesTimestampedArchive()
    {
        string dataDir = Path.Combine(_dir, "data");
        Environment.SetEnvironmentVariable("DATA_DIR", dataDir);
        Directory.CreateDirectory(dataDir);
        string modelPath = Path.Combine(_dir, "model.pt");
        File.WriteAllText(modelPath, "content");

        ModelVersionManager.SaveVersion(_dir, modelPath);

        string modelsDir = Path.Combine(dataDir, "models");
        var dirs = Directory.GetDirectories(modelsDir);
        Assert.Single(dirs);
        string destFile = Path.Combine(dirs[0], "model.pt");
        Assert.True(File.Exists(destFile));
        string name = Path.GetFileName(dirs[0]);
        Assert.Matches(@"\d{8}_\d{6}", name);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("DATA_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
