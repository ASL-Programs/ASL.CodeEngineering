using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowEnvVarTests : IDisposable
{
    private class FailingProvider : IAIProvider
    {
        public string Name => "Failing";
        public Task<string> SendChatAsync(string prompt, System.Threading.CancellationToken cancellationToken = default)
            => throw new Exception("fail");
    }

    private readonly string _root;

    public MainWindowEnvVarTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_root);
    }

    [StaFact]
    public void SendButtonClick_UsesEnvironmentDirectories()
    {
        string dataDir = Path.Combine(_root, "data");
        string kbDir = Path.Combine(_root, "kb");
        string logsDir = Path.Combine(_root, "logs");
        Environment.SetEnvironmentVariable("DATA_DIR", dataDir);
        Environment.SetEnvironmentVariable("KB_DIR", kbDir);
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        var window = new MainWindow();
        var field = typeof(MainWindow).GetField("_aiProvider", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, new FailingProvider());
        window.PromptTextBox.Text = "hi";
        var method = typeof(MainWindow).GetMethod("SendButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        string providerName = PathHelpers.SanitizeFileName("Failing");
        Assert.True(File.Exists(Path.Combine(dataDir, providerName, "chatlog.jsonl")));
        Assert.True(File.Exists(Path.Combine(kbDir, providerName, "summaries.jsonl")));
        Assert.True(Directory.Exists(logsDir) && Directory.GetFiles(logsDir).Length > 0);
        window.Close();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("DATA_DIR", null);
        Environment.SetEnvironmentVariable("KB_DIR", null);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
