using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowLoggingTests : IDisposable
{
    private class TestProvider : IAIProvider
    {
        public string Name => "Test";
        public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
            => Task.FromResult("response");
    }

    private readonly string _root;

    public MainWindowLoggingTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_root);
    }

    [StaFact]
    public void SendButtonClick_ReadOnlyDataDir_LogsError()
    {
        string dataDir = Path.Combine(_root, "data");
        string logsDir = Path.Combine(_root, "logs");
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(logsDir);
        DirectoryInfo info = new DirectoryInfo(dataDir);
        info.Attributes |= FileAttributes.ReadOnly;

        Environment.SetEnvironmentVariable("DATA_DIR", dataDir);
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        var window = new MainWindow();
        var field = typeof(MainWindow).GetField("_aiProvider", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, new TestProvider());

        window.PromptTextBox.Text = "hi";
        var before = Directory.GetFiles(logsDir);
        var method = typeof(MainWindow).GetMethod("SendButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        var after = Directory.GetFiles(logsDir);
        Assert.True(after.Length > before.Length);
        Assert.Equal("Log Error", window.StatusTextBlock.Text);

        info.Attributes &= ~FileAttributes.ReadOnly;
        window.Close();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("DATA_DIR", null);
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
