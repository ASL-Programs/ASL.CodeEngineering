using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using ASL.CodeEngineering;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowPackageTests : IDisposable
{
    private readonly string _dir;

    public MainWindowPackageTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_dir, "packages", "p1"));
        Directory.CreateDirectory(Path.Combine(_dir, "packages", "p2"));
        File.WriteAllText(Path.Combine(_dir, "packages", "p1", "a.md"), "a");
        File.WriteAllText(Path.Combine(_dir, "packages", "p2", "b.md"), "b");
        Environment.SetEnvironmentVariable("KB_DIR", _dir);
    }

    [StaFact]
    public void Constructor_LoadsPackageNames()
    {
        var window = new MainWindow();
        var field = typeof(MainWindow).GetField("_packageNames", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var list = ((System.Collections.ObjectModel.ObservableCollection<string>)field.GetValue(window)!).ToArray();
        Assert.Contains("p1", list);
        Assert.Contains("p2", list);
        window.Close();
    }

    [StaFact]
    public void PackageCheckBoxChanged_UpdatesSelection()
    {
        var window = new MainWindow();
        var method = typeof(MainWindow).GetMethod("PackageCheckBox_Changed", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var dictField = typeof(MainWindow).GetField("_selectedPackages", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var cb = new CheckBox { Content = "p1", IsChecked = false };
        method.Invoke(window, new object?[] { cb, new System.Windows.RoutedEventArgs() });
        var dict = (System.Collections.Generic.Dictionary<string, bool>)dictField.GetValue(window)!;
        Assert.False(dict["p1"]);
        window.Close();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("KB_DIR", null);
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, true);
    }
}
