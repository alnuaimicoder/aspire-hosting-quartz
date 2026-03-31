using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CommunityToolkit.Aspire.Hosting.Quartz;

/// <summary>
/// Extension methods for adding Quartz.NET hosting services to an application.
/// </summary>
public static class QuartzHostingExtensions
{
    /// <summary>
    /// Adds Quartz.NET background job scheduler to the service collection.
    /// This should be called in worker services that process background jobs.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQuartzWorker(this IServiceCollection services)
    {
        // Configure Quartz.NET
        services.AddQuartz(q =>
        {
            q.SchedulerId = "AspireQuartzScheduler";
            q.MaxBatchSize = 10;
        });

        // Add Quartz hosted service
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
