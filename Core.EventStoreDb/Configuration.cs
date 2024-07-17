using Core.Events;
using Core.EventStoreDb.Subscriptions;
using Core.EventStoreDb.Subscriptions.Batch;
using Core.EventStoreDb.Subscriptions.Checkpoints;
using Core.Extensions;
using Core.OpenTelemetry;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.EventStoreDb;

public class EventStoreDbConfig
{
    public string ConnectionString { get; set; } = default!;
}

public record EventStoreDbOptions(bool UseInternalCheckpointing = true);

public static class EventStoreDbConfigExtensions
{
    private const string DefaultConfigKey = "EventStore";

    public static IServiceCollection AddEventStoreDb(
        this IServiceCollection services,
        IConfiguration config,
        EventStoreDbOptions? options = null
    ) =>
        services.AddEventStoreDb(
            config.GetRequiredConfig<EventStoreDbConfig>(DefaultConfigKey),
            options
        );

    public static IServiceCollection AddEventStoreDb(
        this IServiceCollection services,
        EventStoreDbConfig eventStoreDbConfig,
        EventStoreDbOptions? options = null
    )
    {
        services
            .AddSingleton(EventTypeMapper.Instance)
            .AddSingleton(
                new EventStoreClient(EventStoreClientSettings.Create(eventStoreDbConfig.ConnectionString)))
            .AddScoped<EventsBatchProcessor, EventsBatchProcessor>()
            .AddScoped<IEventsBatchCheckpointer, EventsBatchCheckpointer>()
            .AddSingleton<ISubscriptionStoreSetup, NulloSubscriptionStoreSetup>();

        if (options?.UseInternalCheckpointing != false)
        {
            services
                .AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();
        }

        return services.AddHostedService(serviceProvider =>
            {
                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                var coordinator = serviceProvider.GetRequiredService<EventStoreDbSubscriptionsToAllCoordinator>();

                TelemetryPropagator.UseDefaultCompositeTextMapPropagator();

                return new BackgroundWorker<EventStoreDbSubscriptionsToAllCoordinator>(
                    coordinator,
                    logger,
                    (c, ct) => c.SubscribeToAll(ct)
                );
            }
        );
    }

    public static IServiceCollection AddEventStoreDbSubscriptionToAll<THandler>(
        this IServiceCollection services,
        string subscriptionId
    ) where THandler : IEventBatchHandler =>
        services.AddEventStoreDbSubscriptionToAll(
            new EventStoreDbSubscriptionToAllOptions { SubscriptionId = subscriptionId },
            sp => [sp.GetRequiredService<THandler>()]
        );


    public static IServiceCollection AddEventStoreDbSubscriptionToAll<THandler>(
        this IServiceCollection services,
        EventStoreDbSubscriptionToAllOptions subscriptionOptions
    ) where THandler : IEventBatchHandler =>
        services.AddEventStoreDbSubscriptionToAll(subscriptionOptions, sp => [sp.GetRequiredService<THandler>()]);

    public static IServiceCollection AddEventStoreDbSubscriptionToAll(
        this IServiceCollection services,
        EventStoreDbSubscriptionToAllOptions subscriptionOptions,
        Func<IServiceProvider, IEventBatchHandler[]> handlers
    )
    {
        services.AddSingleton<EventStoreDbSubscriptionsToAllCoordinator>();

        return services.AddKeyedSingleton<EventStoreDbSubscriptionToAll>(
            subscriptionOptions.SubscriptionId,
            (sp, _) =>
            {
                var subscription = new EventStoreDbSubscriptionToAll(
                    sp.GetRequiredService<EventStoreClient>(),
                    sp.GetRequiredService<IServiceScopeFactory>(),
                    sp.GetRequiredService<ILogger<EventStoreDbSubscriptionToAll>>()
                ) { Options = subscriptionOptions, GetHandlers = handlers };

                return subscription;
            });
    }
}
