using CommunityToolkit.Aspire.Quartz;
using CommunityToolkit.Aspire.Quartz;
using QuartzSample.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add Quartz client for job enqueuing
builder.Services.AddQuartzClient();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Endpoint to enqueue email job
app.MapPost("/send-email", async (IBackgroundJobClient jobClient, EmailRequest request) =>
{
    var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
        parameters: new { email = request.Email, subject = request.Subject },
        options: new JobOptions
        {
            IdempotencyKey = $"email-{request.Email}-{DateTime.UtcNow:yyyyMMdd}",
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                Strategy = BackoffStrategy.Exponential
            }
        });

    return Results.Ok(new { jobId, message = "Email job enqueued successfully" });
})
.WithName("SendEmail");

// Endpoint to schedule email job with delay
app.MapPost("/schedule-email", async (IBackgroundJobClient jobClient, ScheduleEmailRequest request) =>
{
    var jobId = await jobClient.ScheduleAsync<SendEmailJob>(
        delay: TimeSpan.FromMinutes(request.DelayMinutes),
        parameters: new { email = request.Email, subject = request.Subject },
        options: new JobOptions
        {
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                Strategy = BackoffStrategy.Exponential
            }
        });

    return Results.Ok(new { jobId, message = $"Email job scheduled for {request.DelayMinutes} minutes from now" });
})
.WithName("ScheduleEmail");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record EmailRequest(string Email, string Subject);
record ScheduleEmailRequest(string Email, string Subject, int DelayMinutes);
