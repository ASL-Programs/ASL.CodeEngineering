using System.IO;
using System.Linq;

namespace ASL.CodeEngineering.AI;

public static class PathHelpers
{
    /// <summary>
    /// Replaces invalid file name characters with underscores so the result can
    /// be safely used as part of a path.
    /// </summary>
    public static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return sanitized;
    }
}
