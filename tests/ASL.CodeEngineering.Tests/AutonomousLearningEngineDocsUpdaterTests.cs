using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class AutonomousLearningEngineDocsUpdaterTests
{
    private class StubProvider : IAIProvider
    {
        public string Name => "Stub";
        public bool RequiresNetwork => false;
        public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
            => Task.FromResult("ok");
    }

    [Fact]
    public async Task RunAsync_ArchivesDocsAfterCycle()
    {
        string root = AppContext.BaseDirectory;
        string agentsPath = Path.Combine(root, "AGENTS.md");
        string nextPath = Path.Combine(root, "NEXT_STEPS.md");
        File.WriteAllText(agentsPath, "");
        File.WriteAllText(nextPath, "");

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(6));
        await AutonomousLearningEngine.RunAsync(() => new StubProvider(), cts.Token);

        string archiveDir = Path.Combine(root, "docs", "archive");
        Assert.True(Directory.Exists(archiveDir));
        Assert.Contains(Directory.GetFiles(archiveDir), f => f.Contains("AGENTS_"));
        Assert.Contains(Directory.GetFiles(archiveDir), f => f.Contains("NEXT_STEPS_"));
        string[] agentsLines = File.ReadAllLines(agentsPath);
        Assert.Contains(agentsLines, l => l.Contains("Learning cycle 1 completed"));

        Directory.Delete(archiveDir, true);
        Directory.Delete(Path.Combine(root, "knowledge_base"), true);
        File.Delete(agentsPath);
        File.Delete(nextPath);
    }
}
