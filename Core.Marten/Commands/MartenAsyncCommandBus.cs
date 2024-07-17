using Core.Commands;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Marten.Commands;

public class MartenAsyncCommandBus(IDocumentSession documentSession) : IAsyncCommandBus
{
    public static readonly Guid CommandsStreamId = new("11111111-1111-1111-1111-111111111111");

    public Task Schedule<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : notnull
    {
        documentSession.Events.Append(CommandsStreamId, command);
        return documentSession.SaveChangesAsync(ct);
    }
}

public static class Configuration
{
    public static IServiceCollection AddMartenAsyncCommandBus(this IServiceCollection services) =>
        services.AddScoped<IAsyncCommandBus, MartenAsyncCommandBus>()
            .AddCommandForwarder();
}
