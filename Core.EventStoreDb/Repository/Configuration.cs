using Core.Aggregates;
using Core.OpenTelemetry;
using Core.OptimisticConcurrency;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventStoreDb.Repository;

public static class Configuration
{
    public static IServiceCollection AddEventStoreDbRepository<T>(
        this IServiceCollection services,
        bool withAppendScope = true,
        bool withTelemetry = true
    ) where T : class, IAggregate
    {
        services.AddScoped<IEventStoreDbRepository<T>, EventStoreDbRepository<T>>();

        if (withAppendScope)
        {
            services.Decorate<IEventStoreDbRepository<T>>(
                (inner, sp) => new EventStoreDbRepositoryWithETagDecorator<T>(
                    inner,
                    sp.GetRequiredService<IExpectedResourceVersionProvider>(),
                    sp.GetRequiredService<INextResourceVersionProvider>()
                )
            );
        }

        if (withTelemetry)
        {
            services.Decorate<IEventStoreDbRepository<T>>(
                (inner, sp) => new EventStoreDbRepositoryWithTelemetryDecorator<T>(
                    inner,
                    sp.GetRequiredService<IActivityScope>()
                )
            );
        }

        return services;
    }
}
