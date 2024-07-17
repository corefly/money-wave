using Core.Commands;
using Core.EventStoreDb.Repository;
using Microsoft.Extensions.DependencyInjection;
using MoneyWave.UserAccounts.OpeningUserAccount;

namespace MoneyWave.UserAccounts;

public static class Configuration
{
    public static IServiceCollection AddUserAccounts(this IServiceCollection services) =>
        services.AddEventStoreDbRepository<UserAccount>()
            .AddCommandHandlers()
            .AddProjections()
            .AddQueryHandlers();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<OpenUserAccount, HandleOpenUserAccount>();

    private static IServiceCollection AddProjections(this IServiceCollection services) => services;

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) => services;
}
