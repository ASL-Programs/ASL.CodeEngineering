using System;
using Xunit;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering.Tests;

public class GpuDeviceManagerTests
{
    [Fact]
    public void ListGpus_ReturnsList()
    {
        var list = GpuDeviceManager.ListGpus();
        Assert.NotNull(list);
    }

    [Fact]
    public void UseGpu_DoesNotThrow()
    {
        GpuDeviceManager.UseGpu(null);
    }
}
