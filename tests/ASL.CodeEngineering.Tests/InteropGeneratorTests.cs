using System;
using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class InteropGeneratorTests
{
    [Fact]
    public async Task CreatePythonWrapper_BuildsSuccessfully()
    {
        if (!TestHelpers.ToolExists("dotnet") || !TestHelpers.ToolExists("python"))
            throw new SkipException("dotnet or python not installed");

        var temp = Directory.CreateTempSubdirectory();
        try
        {
            string wrapperPath = InteropGenerator.CreatePythonWrapper("Wrap", temp.FullName);
            var runner = new DotnetBuildTestRunner();
            string result = await runner.BuildAsync(wrapperPath);
            Assert.DoesNotContain("Exit code", result);
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }

    [Fact]
    public async Task CreateNodeWrapper_BuildsSuccessfully()
    {
        if (!TestHelpers.ToolExists("dotnet") || !TestHelpers.ToolExists("node"))
            throw new SkipException("dotnet or node not installed");

        var temp = Directory.CreateTempSubdirectory();
        try
        {
            string wrapperPath = InteropGenerator.CreateNodeWrapper("Wrap", temp.FullName);
            var runner = new DotnetBuildTestRunner();
            string result = await runner.BuildAsync(wrapperPath);
            Assert.DoesNotContain("Exit code", result);
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }
}
