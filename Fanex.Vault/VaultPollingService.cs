namespace Fanex.Vault;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

internal sealed class VaultPollingService : BackgroundService
{
    private readonly VaultOptions vaultOptions;

    private readonly VaultService vaultService;

    private readonly VaultProvider vaultProvider;

    internal VaultPollingService(IOptions<VaultOptions> vaultOptions, VaultService vaultService, VaultProvider vaultProvider)
    {
        this.vaultOptions = vaultOptions.Value;
        this.vaultProvider = vaultProvider;
        this.vaultService = vaultService;
    }

    public Exception LatestException { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var milliseconds = vaultOptions.MinutesPolling * 60 * 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            await SafeReloadVaultValue(stoppingToken).ConfigureAwait(false);
            await Task.Delay(milliseconds, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task SafeReloadVaultValue(CancellationToken stoppingToken)
    {
        const int tryCount = 3;
        for (var i = 0; i < tryCount; i++)
        {
            try
            {
                var vaultMap = await vaultProvider.LoadAllAsync().ConfigureAwait(false);
                vaultService.SaveVaultMap(vaultMap);
                break;
            }
            catch (Exception e)
            {
                LatestException = e;
                await Task.Delay(3000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}