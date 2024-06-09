var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MoneyWave_ApiService>("apiservice");

builder.AddProject<Projects.MoneyWave_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();