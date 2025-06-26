using System;
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
        var providers = AIProviderLoader.LoadProviders(AppContext.BaseDirectory);
        Assert.Contains("Dummy", providers.Keys);
    }

    [StaFact]
    public void MainWindow_ListsRuntimeCompiledProvider()
    {
        var window = new MainWindow();
        var items = window.ProviderComboBox.Items.OfType<string>().ToArray();
        Assert.Contains("Dummy", items);
        window.Close();
    }
}
