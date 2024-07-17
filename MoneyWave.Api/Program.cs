using Microsoft.OpenApi.Models;
using Core;
using Core.Events;
using Core.EventStoreDb;
using Core.Exceptions;
using Core.OpenTelemetry;
using EventStore.Client;
using MoneyWave.Api;
using MoneyWave.Core.WebApis.Middlewares;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyWave", Version = "v1" }); })
    .AddCoreServices()
    .AddDefaultExceptionHandler(
        (exception, _) => exception switch
        {
            AggregateNotFoundException => exception.MapToProblemDetails(StatusCodes.Status404NotFound),
            WrongExpectedVersionException => exception.MapToProblemDetails(StatusCodes.Status412PreconditionFailed),
            _ => null
        })
    .AddEventStoreDbSubscriptionToAll<EventBusBatchHandler>("money-wave")
    .AddUserAccountsModule(builder.Configuration)
    .AddOptimisticConcurrencyMiddleware()
    .AddOpenTelemetry("money-wave", OpenTelemetryOptions.Build(options =>
        options.WithTracing(t =>
            t.AddJaegerExporter()
        ).DisableConsoleExporter(true)
    ))
    .AddControllers();

var app = builder.Build();

app
    .UseHttpsRedirection()
    .UseExceptionHandler()
    .UseOptimisticConcurrencyMiddleware()
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints => endpoints.MapControllers());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Money wave V1");
        c.RoutePrefix = string.Empty;
    });
}

app.Run();
