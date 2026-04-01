using CommunityToolkit.Aspire.Hosting.Quartz;

var builder = DistributedApplication.CreateBuilder(args);

// Option 1: SQL Server for Quartz job storage (commented out)
// var sqlserver = builder.AddSqlServer("sql")
//     .WithLifetime(ContainerLifetime.Persistent)
//     .AddDatabase("quartzdb");

// Option 2: PostgreSQL for Quartz job storage
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .AddDatabase("quartzdb");

// Add API service with Quartz.NET scheduling (all-in-one)
var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice")
    .WithReference(postgres);

// Add web frontend
builder.AddProject<Projects.QuartzSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
