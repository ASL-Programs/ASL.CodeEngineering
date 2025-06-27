using System.IO;

namespace ASL.CodeEngineering.AI;

public static class InteropGenerator
{
    public static string CreatePythonWrapper(string name, string directory)
    {
        var wrapperDir = Path.Combine(directory, name);
        Directory.CreateDirectory(wrapperDir);

        File.WriteAllText(Path.Combine(wrapperDir, $"{name}.csproj"), CsprojTemplate());
        File.WriteAllText(Path.Combine(wrapperDir, "Program.cs"), ProgramTemplate());
        File.WriteAllText(Path.Combine(wrapperDir, "script.py"), "print('hello from python')\n");

        return wrapperDir;
    }

    private static string CsprojTemplate() =>
        "<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
        "  <PropertyGroup>\n" +
        "    <OutputType>Exe</OutputType>\n" +
        "    <TargetFramework>net7.0</TargetFramework>\n" +
        "  </PropertyGroup>\n" +
        "</Project>\n";

    private static string ProgramTemplate() =>
        "using System.Diagnostics;\n" +
        "var psi = new ProcessStartInfo(\"python\", \"script.py\")\n" +
        "{ RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };\n" +
        "using var process = Process.Start(psi)!;\n" +
        "Console.WriteLine(process.StandardOutput.ReadToEnd());\n" +
        "process.WaitForExit();\n";
}
