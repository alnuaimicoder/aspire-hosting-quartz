using CommunityToolkit.Aspire.Quartz;
using Quartz;
using QuartzSample.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Add service defaults
builder.AddServiceDefaults();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.UseSimpleTypeLoader();
    q.UseInMemoryStore();
    q.UseDefaultThreadPool(tp =>
    {
        tp.MaxConcurrency = 10;
    });

    // Use JSON serialization
    q.UsePersistentStore(options =>
    {
        options.UseProperties = true;
        options.UseSqlServer(sqlServer =>
        {
            sqlServer.ConnectionString = builder.Configuration.GetConnectionString("quartzdb");
            sqlServer.TablePrefix = "QRTZ_";
        });
        options.UseJsonSerializer();
    });
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Register job types
builder.Services.AddTransient<SendEmailJob>();

var host = builder.Build();
host.Run();
