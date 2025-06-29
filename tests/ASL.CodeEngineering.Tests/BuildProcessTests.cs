using System;
using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class BuildProcessTests : IDisposable
{
    private readonly string _root;

    private class RecordingRunner : IBuildTestRunner
    {
        public string Name => "Recording";
        public string? BuildPath;
        public Task<string> BuildAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default)
        {
            BuildPath = projectPath;
            return Task.FromResult("built:" + projectPath);
        }
        public Task<string> TestAsync(string projectPath, System.Threading.CancellationToken cancellationToken = default)
            => Task.FromResult("tested:" + projectPath);
    }

    public BuildProcessTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_root, "src"));
        File.WriteAllText(Path.Combine(_root, "src", "file.txt"), "content");
    }

    [Fact]
    public async Task BuildNewVersionAsync_CreatesTimestampedFolderAndRunsBuild()
    {
        string dataDir = Path.Combine(_root, "data");
        Environment.SetEnvironmentVariable("DATA_DIR", dataDir);
        var runner = new RecordingRunner();

        string versionPath = await BuildProcess.BuildNewVersionAsync(_root, runner);

        string versionsDir = Path.Combine(dataDir, "versions");
        var dirs = Directory.GetDirectories(versionsDir);
        Assert.Single(dirs);
        Assert.Equal(dirs[0], versionPath);
        string name = Path.GetFileName(dirs[0]);
        Assert.Matches(@"\d{8}_\d{6}", name);
        Assert.Equal(Path.Combine(versionPath, "src"), runner.BuildPath);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("DATA_DIR", null);
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
