using System;
using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ComplianceCheckerTests : IDisposable
{
    private readonly string _root;

    public ComplianceCheckerTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_root, "data", "profiles"));
    }

    [Fact]
    public void Check_ReturnsCompliantResult()
    {
        File.WriteAllText(Path.Combine(_root, "data", "profiles", "a.json"), "{}");
        var policy = new CompliancePolicy { MaxProfiles = 2 };
        var result = ComplianceChecker.Check(_root, policy);
        Assert.True(result.IsCompliant);
        Assert.Contains("ProfileCount", result.Details);
    }

    [Fact]
    public void ExportAndErase_Works()
    {
        string profile = Path.Combine(_root, "data", "profiles", "a.json");
        File.WriteAllText(profile, "{}");
        string zip = Path.Combine(_root, "out.zip");
        string path = UserDataManager.Export(_root, zip);
        Assert.True(File.Exists(path));
        UserDataManager.Erase(_root);
        Assert.False(File.Exists(profile));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, true);
    }
}
