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

public class MainWindowSummaryLoggingTests : IDisposable
{
    private class TwoCallProvider : IAIProvider
    {
        private int _count;
        public string Name => "TwoCall";
        public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
        {
            _count++;
            if (_count == 2)
                throw new Exception("fail");
            return Task.FromResult("ok");
        }
    }

    private readonly string _root;

    public MainWindowSummaryLoggingTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_root);
    }

    [StaFact]
    public void SendButtonClick_SummaryFailure_LogsError()
    {
        string logsDir = Path.Combine(_root, "logs");
        Directory.CreateDirectory(logsDir);
        Environment.SetEnvironmentVariable("LOGS_DIR", logsDir);

        var window = new MainWindow();
        var field = typeof(MainWindow).GetField("_aiProvider", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, new TwoCallProvider());

        window.PromptTextBox.Text = "hi";
        var before = Directory.GetFiles(logsDir);
        var method = typeof(MainWindow).GetMethod("SendButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        var after = Directory.GetFiles(logsDir);
        Assert.True(after.Length > before.Length);
        Assert.Equal("Summary Error", window.StatusTextBlock.Text);

        window.Close();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("LOGS_DIR", null);
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
