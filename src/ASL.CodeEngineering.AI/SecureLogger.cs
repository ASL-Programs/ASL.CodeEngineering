using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Writes log files optionally encrypted with AES.
/// The encryption key is read from the <c>LOG_ENCRYPTION_KEY</c> environment variable.
/// </summary>
public static class SecureLogger
{
    /// <summary>
    /// Writes content to a timestamped log file under LOGS_DIR or the executable directory.
    /// When <c>LOG_ENCRYPTION_KEY</c> is set, the file is AES encrypted with the IV prepended.
    /// </summary>
    public static void Write(string prefix, string content)
    {
        var logsDir = Environment.GetEnvironmentVariable("LOGS_DIR") ??
                      Path.Combine(AppContext.BaseDirectory, "logs");

        if (!TryWrite(logsDir))
        {
            var fallback = Path.Combine(AppContext.BaseDirectory, "logs");
            if (fallback != logsDir)
                TryWrite(fallback);
        }

        bool TryWrite(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
                WriteFile(file, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    private static void WriteFile(string file, string content)
    {
        var keyString = Environment.GetEnvironmentVariable("LOG_ENCRYPTION_KEY");
        if (string.IsNullOrWhiteSpace(keyString))
        {
            File.WriteAllText(file, content);
            return;
        }

        try
        {
            byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(keyString));
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(aes.IV, 0, aes.IV.Length);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(content);
        }
        catch
        {
            // fallback to plain write on failure
            File.WriteAllText(file, content);
        }
    }
}
