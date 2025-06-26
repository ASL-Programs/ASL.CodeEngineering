using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

[CollectionDefinition("PluginFixture")]
public class PluginFixtureCollection : ICollectionFixture<CompilePluginFixture> { }

[Collection("PluginFixture")]
public class PluginLoaderTests
{
    private readonly CompilePluginFixture _fixture;

    public PluginLoaderTests(CompilePluginFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Loader_DetectsRuntimeCompiledPlugins()
    {
        var analyzers = PluginLoader.LoadAnalyzers(_fixture.BaseDirectory);
        var runners = PluginLoader.LoadRunners(_fixture.BaseDirectory);
        Assert.Contains("DummyAnalyzer", analyzers.Keys);
        Assert.Contains("DummyRunner", runners.Keys);
    }

    [StaFact]
    public void MainWindow_ListsRuntimeCompiledPlugins()
    {
        var window = new MainWindow();
        var analyzers = PluginLoader.LoadAnalyzers(_fixture.BaseDirectory);
        var runners = PluginLoader.LoadRunners(_fixture.BaseDirectory);

        var fieldA = typeof(MainWindow).GetField("_analyzerFactories", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var fieldR = typeof(MainWindow).GetField("_runnerFactories", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var analyzerFactories = (Dictionary<string, Func<IAnalyzerPlugin>>)fieldA.GetValue(window)!;
        var runnerFactories = (Dictionary<string, Func<ICodeRunnerPlugin>>)fieldR.GetValue(window)!;
        foreach (var pair in analyzers)
            if (!analyzerFactories.ContainsKey(pair.Key))
                analyzerFactories[pair.Key] = pair.Value;
        foreach (var pair in runners)
            if (!runnerFactories.ContainsKey(pair.Key))
                runnerFactories[pair.Key] = pair.Value;
        window.AnalyzerComboBox.ItemsSource = analyzerFactories.Keys;
        window.RunnerComboBox.ItemsSource = runnerFactories.Keys;

        Assert.Contains("DummyAnalyzer", window.AnalyzerComboBox.Items.OfType<string>());
        Assert.Contains("DummyRunner", window.RunnerComboBox.Items.OfType<string>());
        window.Close();
    }
}
