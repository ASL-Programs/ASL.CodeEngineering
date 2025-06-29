using System;
using System.IO;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

public class CompliancePolicy
{
    public int MaxProfiles { get; set; } = 5;
    public bool RequireLogEncryption { get; set; }
}

public class ComplianceResult
{
    public bool IsCompliant { get; set; }
    public string Details { get; set; } = string.Empty;
}

public static class ComplianceChecker
{
    public static ComplianceResult Check(string projectRoot, CompliancePolicy policy)
    {
        string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                           Path.Combine(projectRoot, "data");
        string profilesDir = Path.Combine(baseData, "profiles");
        int profileCount = Directory.Exists(profilesDir)
            ? Directory.GetFiles(profilesDir, "*.json").Length
            : 0;

        bool logsEncrypted = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LOG_ENCRYPTION_KEY"));

        bool ok = profileCount <= policy.MaxProfiles &&
                  (!policy.RequireLogEncryption || logsEncrypted);

        var info = new
        {
            ProfileCount = profileCount,
            policy.MaxProfiles,
            LogsEncrypted = logsEncrypted,
            policy.RequireLogEncryption,
            Compliant = ok
        };
        string details = JsonSerializer.Serialize(info);
        try { SecureLogger.Write("Compliance", details); } catch { }

        return new ComplianceResult { IsCompliant = ok, Details = details };
    }
}
