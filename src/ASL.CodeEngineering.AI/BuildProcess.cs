using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public static class BuildProcess
{
    public static async Task<string> BuildNewVersionAsync(string projectRoot, IBuildTestRunner runner, CancellationToken cancellationToken = default)
    {
        string versionPath = VersionManager.SaveVersion(projectRoot);
        string srcPath = Path.Combine(versionPath, "src");
        await runner.BuildAsync(srcPath, cancellationToken);
        try
        {
            DocsUpdater.GenerateReleaseReport(projectRoot, versionPath);
        }
        catch
        {
            // ignore report failures
        }
        return versionPath;
    }
}
