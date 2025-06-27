using System;
using System.Linq;
using System.Windows;
using ASL.CodeEngineering;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowOfflineTests
{
    [StaFact]
    public void OfflineMode_RemovesNetworkProviders()
    {
        Environment.SetEnvironmentVariable("DISABLE_NETWORK_PROVIDERS", "1");
        var window = new MainWindow();
        var items = window.ProviderComboBox.Items.OfType<string>().ToArray();
        Assert.DoesNotContain("OpenAI", items);
        Assert.Contains("Echo", items);
        Assert.Contains("Reverse", items);
        window.Close();
        Environment.SetEnvironmentVariable("DISABLE_NETWORK_PROVIDERS", null);
    }
}
