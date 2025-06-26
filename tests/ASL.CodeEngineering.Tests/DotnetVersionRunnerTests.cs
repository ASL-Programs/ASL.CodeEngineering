using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;
using Xunit.Sdk;

namespace ASL.CodeEngineering.Tests;

public class DotnetVersionRunnerTests
{
    [Fact]
    public async Task RunAsync_ReturnsVersion()
    {
        if (!TestHelpers.ToolExists("dotnet"))
            throw new SkipException("dotnet not installed");

        var runner = new DotnetVersionRunner();
        string result = await runner.RunAsync(AppContext.BaseDirectory);
        Assert.Matches(@"\d+(\.\d+)+", result);
    }
}
