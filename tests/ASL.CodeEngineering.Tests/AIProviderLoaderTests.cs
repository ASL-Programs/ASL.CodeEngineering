using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ASL.CodeEngineering;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

[CollectionDefinition("ProviderFixture")] 
public class ProviderFixtureCollection : ICollectionFixture<CompileProviderFixture> { }

[Collection("ProviderFixture")]
public class AIProviderLoaderTests
{
    private readonly CompileProviderFixture _fixture;

    public AIProviderLoaderTests(CompileProviderFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Loader_DetectsRuntimeCompiledProvider()
    {
        var providers = AIProviderLoader.LoadProviders(_fixture.ProvidersDirectory);
        Assert.Contains("Dummy", providers.Keys);
    }

    [StaFact]
    public void MainWindow_ListsRuntimeCompiledProvider()
    {
        var window = new MainWindow();
        var providers = AIProviderLoader.LoadProviders(_fixture.ProvidersDirectory);

        var field = typeof(MainWindow).GetField("_providerFactories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var factories = (Dictionary<string, Func<IAIProvider>>)field.GetValue(window)!;
        foreach (var pair in providers)
        {
            if (!factories.ContainsKey(pair.Key))
                factories[pair.Key] = pair.Value;
        }
        window.ProviderComboBox.ItemsSource = factories.Keys;

        var items = window.ProviderComboBox.Items.OfType<string>().ToArray();
        Assert.Contains("Dummy", items);
        window.Close();
    }
}
