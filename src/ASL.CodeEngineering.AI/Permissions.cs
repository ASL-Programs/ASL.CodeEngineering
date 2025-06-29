using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ASL.CodeEngineering.AI;

public enum Role
{
    Viewer,
    Operator,
    Admin
}

public static class Permissions
{
    private static readonly Dictionary<string, Role> _assignments = new();
    public static string CurrentUser { get; private set; } = string.Empty;
    public static Role CurrentRole { get; private set; } = Role.Viewer;

    public static void Load(string projectRoot)
    {
        string configsDir = Path.Combine(projectRoot, "configs");
        string file = Path.Combine(configsDir, "permissions.enc");
        if (!File.Exists(file))
            return;
        string? key = Environment.GetEnvironmentVariable("PERMISSIONS_KEY");
        if (string.IsNullOrWhiteSpace(key))
            return;
        try
        {
            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            using var fs = File.OpenRead(file);
            byte[] iv = new byte[16];
            fs.Read(iv, 0, iv.Length);
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;
            using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(cs) ?? new();
            _assignments.Clear();
            foreach (var kv in dict)
            {
                if (Enum.TryParse<Role>(kv.Value, out var r))
                    _assignments[kv.Key] = r;
            }
        }
        catch
        {
            // ignore invalid permissions file
        }
    }

    public static void Save(string projectRoot)
    {
        string? key = Environment.GetEnvironmentVariable("PERMISSIONS_KEY");
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("PERMISSIONS_KEY not set");
        string configsDir = Path.Combine(projectRoot, "configs");
        Directory.CreateDirectory(configsDir);
        string file = Path.Combine(configsDir, "permissions.enc");
        byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();
        using var fs = File.Create(file);
        fs.Write(aes.IV, 0, aes.IV.Length);
        using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
        var dict = new Dictionary<string, string>();
        foreach (var kv in _assignments)
            dict[kv.Key] = kv.Value.ToString();
        JsonSerializer.Serialize(cs, dict);
    }

    public static void AssignRole(string user, Role role)
    {
        _assignments[user] = role;
    }

    public static Role GetRole(string user)
        => _assignments.TryGetValue(user, out var role) ? role : Role.Viewer;

    public static void SetCurrentUser(string user)
    {
        CurrentUser = user;
        CurrentRole = GetRole(user);
    }

    public static void SetCurrentRole(Role role)
    {
        CurrentRole = role;
    }

    public static void Require(Role required)
    {
        if (CurrentRole < required)
            throw new UnauthorizedAccessException($"Role {CurrentRole} insufficient for {required}");
    }
}
