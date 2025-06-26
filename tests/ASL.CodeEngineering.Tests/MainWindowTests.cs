using System.Reflection;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class MainWindowTests
{
    [StaFact]
    public void ProviderSelection_UpdatesActiveProvider()
    {
        var window = new MainWindow();

        var field = typeof(MainWindow).GetField("_aiProvider", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.IsType<EchoAIProvider>(field.GetValue(window));

        window.ProviderComboBox.SelectedItem = "OpenAI";
        Assert.IsType<OpenAIProvider>(field.GetValue(window));

        window.ProviderComboBox.SelectedItem = "Echo";
        Assert.IsType<EchoAIProvider>(field.GetValue(window));
        window.Close();
    }
}
