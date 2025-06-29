using System;
using System.IO;
using System.Linq;
using System.Windows;
using ASL.CodeEngineering;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class UserProfileTests : IDisposable
{
    private readonly string _dir;

    public UserProfileTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_dir);
    }

    [StaFact]
    public void ProviderSelection_SavesProfile()
    {
        Environment.SetEnvironmentVariable("DATA_DIR", _dir);
        var window = new MainWindow();
        if (!window.ProviderComboBox.Items.OfType<string>().Contains("OpenAI"))
        {
            window.Close();
            Environment.SetEnvironmentVariable("DATA_DIR", null);
            return;
        }
        window.ProviderComboBox.SelectedItem = "OpenAI";
        window.Close();

        string path = Path.Combine(_dir, "profiles", "Default.json");
        Assert.True(File.Exists(path));
        string json = File.ReadAllText(path);
        Assert.Contains("OpenAI", json);
        Environment.SetEnvironmentVariable("DATA_DIR", null);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
