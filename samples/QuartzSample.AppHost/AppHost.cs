using CommunityToolkit.Aspire.Hosting.Quartz;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server for Quartz job storage
var sqlserver = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("quartzdb");

// Add Quartz background job scheduler
var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

var apiService = builder.AddProject("apiservice", "../QuartzSample.ApiService/QuartzSample.ApiService.csproj")
    .WithReference(quartz);

builder.AddProject("webfrontend", "../QuartzSample.Web/QuartzSample.Web.csproj")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
