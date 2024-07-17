using Core.OptimisticConcurrency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;

namespace MoneyWave.Core.WebApis.Middlewares;

public class OptimisticConcurrencyMiddleware(RequestDelegate next)
{
    private readonly string[] _supportedMethods =
        [HttpMethod.Post.Method, HttpMethod.Put.Method, HttpMethod.Delete.Method];

    public async Task Invoke(
        HttpContext context,
        IExpectedResourceVersionProvider expectedResourceVersionProvider,
        INextResourceVersionProvider nextResourceVersionProvider
    )
    {
        TryGetExpectedVersionFromRequestIfMatchHeader(context, expectedResourceVersionProvider);

        // It's needed to do it in the event handler,
        // as headers cannot be modified after the header was
        context.Response.OnStarting(() =>
        {
            TrySetETagResponseHeader(context, nextResourceVersionProvider);
            return Task.CompletedTask;
        });

        await next(context).ConfigureAwait(false);
    }

    private void TryGetExpectedVersionFromRequestIfMatchHeader(
        HttpContext context,
        IExpectedResourceVersionProvider expectedResourceVersionProvider
    )
    {
        if (!_supportedMethods.Contains(context.Request.Method)) return;

        var ifMatchHeader = context.GetIfMatchRequestHeader();

        if (ifMatchHeader == null || Equals(ifMatchHeader, EntityTagHeaderValue.Any)) return;

        if (!expectedResourceVersionProvider.TrySet(ifMatchHeader.GetSanitizedValue()))
            throw new ArgumentOutOfRangeException(nameof(ifMatchHeader), "Invalid format of If-Match header value");
    }

    private static void TrySetETagResponseHeader(
        HttpContext context,
        INextResourceVersionProvider nextResourceVersionProvider
    )
    {
        var nextExpectedVersion = nextResourceVersionProvider.Value;
        if (nextExpectedVersion == null) return;

        context.TrySetETagResponseHeader(nextExpectedVersion);
    }
}

public static class OptimisticConcurrencyMiddlewareConfig
{
    public static IServiceCollection AddOptimisticConcurrencyMiddleware(this IServiceCollection services)
    {
        services.TryAddScoped<IExpectedResourceVersionProvider, ExpectedResourceVersionProvider>();
        services.TryAddScoped<INextResourceVersionProvider, NextResourceVersionProvider>();

        return services;
    }

    public static IApplicationBuilder UseOptimisticConcurrencyMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<OptimisticConcurrencyMiddleware>();
}
