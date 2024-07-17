using Microsoft.Extensions.DependencyInjection;

namespace Core.EventStoreDb.Subscriptions.Checkpoints.Postgres;

public static class Configuration
{
    public static IServiceCollection AddPostgresCheckpointing(this IServiceCollection services) =>
        services
            .AddScoped<ISubscriptionCheckpointRepository, PostgresSubscriptionCheckpointRepository>()
            .AddSingleton<ISubscriptionStoreSetup, PostgresSubscriptionCheckpointSetup>();
}
