using System.Collections.Generic;

namespace ASL.CodeEngineering;

public class UserProfile
{
    public string Name { get; set; } = "Default";
    public string? Provider { get; set; }
    public string? Analyzer { get; set; }
    public string? Runner { get; set; }
    public string? BuildTestRunner { get; set; }
    public string LastProject { get; set; } = string.Empty;
    public List<string> RecentProjects { get; set; } = new();
}
