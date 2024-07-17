using Core.Events;
using Core.Ids;
using Core.OpenTelemetry;
using Core.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Core.Commands;
using Core.Extensions;
using Core.Queries;

namespace Core;

public static class Configuration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services
            .AllowResolvingKeyedServicesAsDictionary()
            .AddSingleton(TimeProvider.System)
            .AddSingleton(ActivityScope.Instance)
            .AddEventBus()
            .AddInMemoryCommandBus()
            .AddQueryBus();

        services.TryAddScoped<IExternalCommandBus, ExternalCommandBus>();

        services.TryAddScoped<IIdGenerator, GuidIdGenerator>();
        services.TryAddSingleton(EventTypeMapper.Instance);

        return services;
    }
}
