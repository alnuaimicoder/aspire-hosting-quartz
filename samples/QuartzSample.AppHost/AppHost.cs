using CommunityToolkit.Aspire.Hosting.Quartz;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server for Quartz job storage
var sqlserver = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("quartzdb");

// Add Quartz background job scheduler
var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

// Add worker service that processes jobs
var worker = builder.AddProject<Projects.QuartzSample_Worker>("worker")
    .WithReference(sqlserver);

// Add API service that enqueues jobs
var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice")
    .WithReference(quartz);

// Add web frontend
builder.AddProject<Projects.QuartzSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
