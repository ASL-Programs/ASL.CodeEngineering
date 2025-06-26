using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowBuildTestRunnerTests
{
    private class RecordingRunner : IBuildTestRunner
    {
        public string Name => "RecordingBuildRunner";
        public string? BuildPath;
        public string? TestPath;
        public Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
        {
            BuildPath = projectPath;
            return Task.FromResult("build:" + projectPath);
        }
        public Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
        {
            TestPath = projectPath;
            return Task.FromResult("test:" + projectPath);
        }
    }

    [StaFact]
    public void BuildButtonClick_UsesProjectRoot()
    {
        var runner = new RecordingRunner();
        var window = new MainWindow();

        var field = typeof(MainWindow).GetField("_buildTestRunner", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, runner);

        string root = Path.Combine(Path.GetTempPath(), "projroot_" + Path.GetRandomFileName());
        Directory.CreateDirectory(root);
        var rootField = typeof(MainWindow).GetField("_projectRoot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        rootField.SetValue(window, root);

        var method = typeof(MainWindow).GetMethod("BuildButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        Assert.Equal(root, runner.BuildPath);
        window.Close();
    }

    [StaFact]
    public void TestButtonClick_UsesProjectRoot()
    {
        var runner = new RecordingRunner();
        var window = new MainWindow();

        var field = typeof(MainWindow).GetField("_buildTestRunner", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, runner);

        string root = Path.Combine(Path.GetTempPath(), "projroot_" + Path.GetRandomFileName());
        Directory.CreateDirectory(root);
        var rootField = typeof(MainWindow).GetField("_projectRoot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        rootField.SetValue(window, root);

        var method = typeof(MainWindow).GetMethod("TestButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        Assert.Equal(root, runner.TestPath);
        window.Close();
    }
}
