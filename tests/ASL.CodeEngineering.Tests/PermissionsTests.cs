using System;
using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class PermissionsTests : IDisposable
{
    private readonly string _root;

    public PermissionsTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_root);
    }

    [Fact]
    public void VersionRestore_RequiresOperator()
    {
        Permissions.SetCurrentRole(Role.Viewer);
        Assert.Throws<UnauthorizedAccessException>(() => VersionManager.RestoreLatest(_root));
    }

    [Fact]
    public void PluginLoader_RequiresOperator()
    {
        Permissions.SetCurrentRole(Role.Viewer);
        Assert.Throws<UnauthorizedAccessException>(() => AIProviderLoader.LoadProviders(_root));
    }

    [Fact]
    public async Task ExternalCall_RequiresAdmin()
    {
        Permissions.SetCurrentRole(Role.Viewer);
        var provider = new OpenAIProvider();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => provider.SendChatAsync("hi"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
