namespace Fanex.Vault;

using System.Collections.Generic;

public class VaultPath
{
    public VaultPath(string path, string key)
    {
        Path = path;
        Key = key;
    }

    public string Path { get; }

    public string Key { get; }

    public override bool Equals(object obj)
    {
        if (obj is not VaultPath objVault)
        {
            return false;
        }

        return Path == objVault.Path && Key == objVault.Key;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode() * Key.GetHashCode();
    }
}

public class VaultOptions
{
    public string Address { get; set; }

    public string Role { get; set; }

    public string SecretId { get; set; }

    public string MountPath { get; set; }

    public bool Enable { get; set; } = false;

    public int MinutesPolling { get; set; } = 1;

    public List<VaultPath> PathList { get; set; } = [];
}