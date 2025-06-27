using System;
using System.Linq;
using System.Windows;
using ASL.CodeEngineering;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowLocalProviderTests
{
    [StaFact]
    public void Constructor_ListsLocalProvider_WhenOnline()
    {
        Environment.SetEnvironmentVariable("DISABLE_NETWORK_PROVIDERS", null);
        var window = new MainWindow();
        var items = window.ProviderComboBox.Items.OfType<string>().ToArray();
        Assert.Contains("Local", items);
        window.Close();
    }
}
