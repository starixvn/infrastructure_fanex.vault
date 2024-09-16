using Microsoft.Extensions.DependencyInjection;

namespace Fanex.Vault;

using Microsoft.Extensions.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFanexVault(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VaultOptions>(configuration.GetSection("VaultOptions"));
        services.AddSingleton<VaultService, VaultServiceImp>();
        services.AddSingleton<VaultProvider, VaultProviderImp>();
        services.AddSingleton<VaultPollingService>();
        services.AddHostedService<VaultPollingService>();

        return services;
    }
}