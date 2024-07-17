using Core.Commands;
using Core.EventStoreDb.Events;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventStoreDb.Commands;

public class EventStoreDbAsyncCommandBus(EventStoreClient eventStoreClient): IAsyncCommandBus
{
    public static readonly string CommandsStreamId = "commands-external";

    public Task Schedule<TCommand>(TCommand command, CancellationToken ct = default) where TCommand: notnull =>
        eventStoreClient.Append(CommandsStreamId, command, ct);
}

public static class Config
{
    public static IServiceCollection AddEventStoreDbAsyncCommandBus(this IServiceCollection services) =>
        services.AddScoped<IAsyncCommandBus, EventStoreDbAsyncCommandBus>()
            .AddCommandForwarder();
}
