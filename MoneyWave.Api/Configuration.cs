using Core.EventStoreDb;
using Core.Marten;
using MoneyWave.UserAccounts;

namespace MoneyWave.Api;

public static class Configuration
{
    internal static IServiceCollection AddUserAccountsModule(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddMarten(configuration, configKey: "Marten", disableAsyncDaemon: true)
            .Services
            .AddUserAccounts()
            .AddEventStoreDb(configuration);
}
