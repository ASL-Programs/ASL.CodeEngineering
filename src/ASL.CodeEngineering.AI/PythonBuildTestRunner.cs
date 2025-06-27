using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public class PythonBuildTestRunner : IBuildTestRunner
{
    public string Name => "Python";

    public async Task<string> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(projectPath, "*.py", SearchOption.AllDirectories);
        if (files.Length == 0)
            return "No Python files";
        var args = "-m py_compile " + string.Join(' ', files.Select(f => $"\"{f}\""));
        return await ProcessRunner.RunAsync("python", args, projectPath, "pybuild", cancellationToken);
    }

    public async Task<string> TestAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        return await ProcessRunner.RunAsync("pytest", string.Empty, projectPath, "pytest", cancellationToken);
    }
}
