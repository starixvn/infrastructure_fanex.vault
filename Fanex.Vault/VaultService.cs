namespace Fanex.Vault;

using System.Collections.Generic;

public interface VaultService
{
    void SaveVaultMap(Dictionary<VaultPath, string> map);

    string Get(string path, string key);

    string Get(VaultPath path);
}

public class VaultServiceImp : VaultService
{
    private Dictionary<VaultPath, string> vaultMap;

    public void SaveVaultMap(Dictionary<VaultPath, string> map)
    {
        vaultMap = map;
    }

    public string Get(string path, string key)
    {
        var vaultPath = new VaultPath(path, key);
        return Get(vaultPath);
    }

    public string Get(VaultPath path)
    {
        return vaultMap.GetValueOrDefault(path, string.Empty);
    }
}