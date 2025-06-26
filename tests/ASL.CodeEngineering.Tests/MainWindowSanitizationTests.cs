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

public class MainWindowSanitizationTests
{
    private class BadNameProvider : IAIProvider
    {
        public string Name => "Bad:Name";
        public Task<string> SendChatAsync(string prompt, CancellationToken cancellationToken = default)
            => Task.FromResult("response");
    }

    [StaFact]
    public void SendButtonClick_CreatesSanitizedDirectories()
    {
        var provider = new BadNameProvider();
        var window = new MainWindow();
        var field = typeof(MainWindow).GetField("_aiProvider", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(window, provider);

        window.PromptTextBox.Text = "hi";
        var method = typeof(MainWindow).GetMethod("SendButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(window, new object?[] { null, new RoutedEventArgs() });

        string sanitized = PathHelpers.SanitizeFileName(provider.Name);
        string dataDir = Path.Combine(AppContext.BaseDirectory, "data", sanitized);
        string kbDir = Path.Combine(AppContext.BaseDirectory, "knowledge_base", sanitized);
        Assert.True(Directory.Exists(dataDir));
        Assert.True(Directory.Exists(kbDir));

        if (Directory.Exists(dataDir)) Directory.Delete(dataDir, true);
        if (Directory.Exists(kbDir)) Directory.Delete(kbDir, true);
        window.Close();
    }
}
