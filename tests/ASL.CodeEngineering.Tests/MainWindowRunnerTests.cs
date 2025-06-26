using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowRunnerTests
{
    private class RecordingRunner : ICodeRunnerPlugin
    {
        public string Name => "RecordingRunner";
        public string? ReceivedPath;
        public Task<string> RunAsync(string path, CancellationToken cancellationToken = default)
        {
            ReceivedPath = path;
            return Task.FromResult("ran:" + path);
        }
    }

    [StaFact]
    public void RunButtonClick_UsesProjectRoot()
    {
        var runner = new RecordingRunner();
        var window = new MainWindow();

        var runnerField = typeof(MainWindow).GetField("_runner", BindingFlags.NonPublic | BindingFlags.Instance)!;
        runnerField.SetValue(window, runner);

        string root = Path.Combine(Path.GetTempPath(), "projroot_" + Path.GetRandomFileName());
        Directory.CreateDirectory(root);
        var rootField = typeof(MainWindow).GetField("_projectRoot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        rootField.SetValue(window, root);

        var method = typeof(MainWindow).GetMethod("RunButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        Assert.Equal(root, runner.ReceivedPath);
        window.Close();
    }
}
