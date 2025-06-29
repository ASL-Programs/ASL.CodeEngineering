using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Helper class for listing available GPU devices and selecting one for training.
/// </summary>
public static class GpuDeviceManager
{
    /// <summary>
    /// Returns the names of available GPU devices reported by TensorFlow.NET.
    /// </summary>
    public static IReadOnlyList<string> ListGpus()
    {
        try
        {
            var devices = tf.config.list_physical_devices("GPU");
            return devices.Select(d => d.ToString()).ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Sets the visible GPU device. When <paramref name="name"/> is null or
    /// no matching device is found, TensorFlow uses the default device list.
    /// </summary>
    public static void UseGpu(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        try
        {
            var devices = tf.config.list_physical_devices("GPU");
            var match = devices.FirstOrDefault(d => d.ToString().Contains(name, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                tf.config.set_visible_devices(match, "GPU");
        }
        catch
        {
            // ignore GPU selection failures
        }
    }
}
