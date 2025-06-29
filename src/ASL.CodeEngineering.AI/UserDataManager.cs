using System;
using System.IO;
using System.IO.Compression;

namespace ASL.CodeEngineering.AI;

public static class UserDataManager
{
    public static string Export(string projectRoot, string outputFile)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                           Path.Combine(projectRoot, "data");
        if (File.Exists(outputFile))
            File.Delete(outputFile);
        ZipFile.CreateFromDirectory(baseData, outputFile);
        return outputFile;
    }

    public static void Erase(string projectRoot)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                           Path.Combine(projectRoot, "data");
        if (Directory.Exists(baseData))
            Directory.Delete(baseData, true);
    }
}
