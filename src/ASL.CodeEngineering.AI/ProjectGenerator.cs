using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public static class ProjectGenerator
{
    public static async Task<string> GenerateAsync(string description, string language, string projectsDir, CancellationToken token = default)
    {
        string name = PathHelpers.SanitizeFileName(description.Replace(" ", "_"));
        string projectPath = Path.Combine(projectsDir, name);
        Directory.CreateDirectory(projectPath);

        await File.WriteAllTextAsync(Path.Combine(projectPath, "README.md"), $"# {description}\n", token);

        if (language.Equals("C#", StringComparison.OrdinalIgnoreCase))
        {
            string srcDir = Path.Combine(projectPath, "src");
            Directory.CreateDirectory(srcDir);
            await File.WriteAllTextAsync(Path.Combine(srcDir, $"{name}.csproj"), CsprojTemplate(), token);
            await File.WriteAllTextAsync(Path.Combine(srcDir, "Program.cs"), "Console.WriteLine(\"Hello\");", token);
        }
        else if (language.Equals("Python", StringComparison.OrdinalIgnoreCase))
        {
            await File.WriteAllTextAsync(Path.Combine(projectPath, "main.py"), "print('hello')\n", token);
        }
        else if (language.Equals("Node", StringComparison.OrdinalIgnoreCase) || language.Equals("JavaScript", StringComparison.OrdinalIgnoreCase))
        {
            await File.WriteAllTextAsync(Path.Combine(projectPath, "index.js"), "console.log('hello');\n", token);
        }
        else
        {
            await File.WriteAllTextAsync(Path.Combine(projectPath, "notes.txt"), $"Language {language} not supported yet", token);
        }

        return projectPath;
    }

    private static string CsprojTemplate() =>
        "<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
        "  <PropertyGroup>\n" +
        "    <OutputType>Exe</OutputType>\n" +
        "    <TargetFramework>net7.0</TargetFramework>\n" +
        "  </PropertyGroup>\n" +
        "</Project>\n";
}
