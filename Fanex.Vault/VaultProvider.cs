namespace Fanex.Vault;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

internal interface VaultProvider
{
    Task<Dictionary<VaultPath, string>> LoadAllAsync();

    void ResetVaultToken();

    Task EnsureVaultTokenIsValid();
}

internal class VaultProviderImp : VaultProvider
{
    private const int MinTimeToLive = 15;

    private readonly VaultOptions vaultOptions;

    private readonly VaultClient vaultClient;

    internal VaultProviderImp(IOptions<VaultOptions> vaultOptions)
    {
        this.vaultOptions = vaultOptions.Value;
        vaultClient = CreateVaultClient(this.vaultOptions);
    }

    public async Task<Dictionary<VaultPath, string>> LoadAllAsync()
    {
        var vaultMap = new Dictionary<VaultPath, string>();
        foreach (var path in vaultOptions.PathList)
        {
            var value = await LoadAsync(path);
            vaultMap.Add(path, value);
        }

        return vaultMap;
    }

    private async Task<string> LoadAsync(VaultPath vaultPath)
    {
        var secrets = await vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretAsync(vaultPath.Path, null, vaultOptions.MountPath)
            .ConfigureAwait(false);
        return secrets.Data.Data.TryGetValue(vaultPath.Key, out var value)
            ? value.ToString()
            : string.Empty;
    }


    public async Task EnsureVaultTokenIsValid()
    {
        var tokenData = await vaultClient.V1.Auth.Token.LookupSelfAsync();
        var timeToLive = tokenData.Data.TimeToLive;
        if (timeToLive < MinTimeToLive)
        {
            vaultClient.V1.Auth.ResetVaultToken();
        }
    }

    private static VaultClient CreateVaultClient(VaultOptions vaultOptions)
    {
        var vaultClientSettings = new VaultClientSettings(
            vaultOptions.Address,
            new AppRoleAuthMethodInfo(vaultOptions.Role, vaultOptions.SecretId)
        );

        return new VaultClient(vaultClientSettings);
    }

    public void ResetVaultToken()
    {
        vaultClient.V1.Auth.ResetVaultToken();
    }
}