using Quartz;

namespace QuartzSample.Worker;

[DisallowConcurrentExecution]
public class SendEmailJob : IJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var email = dataMap.GetString("email") ?? "unknown@example.com";
        var subject = dataMap.GetString("subject") ?? "No Subject";
        var jobId = context.FireInstanceId;
        var retryAttempt = context.RefireCount;

        _logger.LogInformation(
            "Executing job: Sending email to {Email} with subject '{Subject}' (JobId: {JobId}, Attempt: {Attempt})",
            email, subject, jobId, retryAttempt);

        // Simulate email sending
        await Task.Delay(1000, context.CancellationToken);

        _logger.LogInformation("Email sent successfully to {Email}", email);
    }
}
