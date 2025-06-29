using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using ASL.CodeEngineering.AI.OfflineLearning;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class AutonomousLearningEngineAdaptiveTests
{
    private class ConstProvider : IAIProvider
    {
        public string Name => "Const";
        public bool RequiresNetwork => false;
        public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
            => Task.FromResult(new string('x', 10));
    }

    [Fact]
    public async Task RunAsync_RecordsMetricsAndAdjustsParameters()
    {
        string root = AppContext.BaseDirectory;
        string agentsPath = Path.Combine(root, "AGENTS.md");
        string nextPath = Path.Combine(root, "NEXT_STEPS.md");
        File.WriteAllText(agentsPath, string.Empty);
        File.WriteAllText(nextPath, string.Empty);
        string pkgDir = Path.Combine(root, "knowledge_base", "packages", "software_languages");
        Directory.CreateDirectory(pkgDir);
        File.WriteAllText(Path.Combine(pkgDir, "a.md"), "a");
        string learnDir = Path.Combine(root, "knowledge_base", "packages", "learn_to_learn");
        Directory.CreateDirectory(learnDir);
        File.WriteAllText(Path.Combine(learnDir, "b.md"), "b");

        var model = new OfflineModel(1);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(6));
        await AutonomousLearningEngine.RunAsync(() => new ConstProvider(), cts.Token,
                                                new[] { "software_languages" }, model, null, 0.01);

        string metricsFile = Path.Combine(root, "knowledge_base", "meta", "training_metrics.jsonl");
        Assert.True(File.Exists(metricsFile));
        Assert.NotEqual(0.01, AutonomousLearningEngine.LastLearningRate);
        Assert.Contains("learn_to_learn", AutonomousLearningEngine.LastPackages);

        Directory.Delete(Path.Combine(root, "knowledge_base"), true);
        Directory.Delete(Path.Combine(root, "docs"), true);
        File.Delete(agentsPath);
        File.Delete(nextPath);
    }
}
