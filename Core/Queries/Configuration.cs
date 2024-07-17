using Microsoft.Extensions.DependencyInjection;

namespace Core.Queries;

public static class Configuration
{
    public static IServiceCollection AddQueryHandler<TQuery, TQueryResult, TQueryHandler>(
        this IServiceCollection services)
        where TQuery : notnull
        where TQueryHandler : class, IQueryHandler<TQuery, TQueryResult> =>
        services
            .AddTransient<TQueryHandler>()
            .AddTransient<IQueryHandler<TQuery, TQueryResult>>(sp => sp.GetRequiredService<TQueryHandler>());
}
