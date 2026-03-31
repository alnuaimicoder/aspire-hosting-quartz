using CommunityToolkit.Aspire.Quartz;
using FluentAssertions;

namespace CommunityToolkit.Aspire.Quartz.Abstractions.Tests;

public class JobOptionsTests
{
    [Fact]
    public void JobOptions_WithIdempotencyKey_ShouldStoreKey()
    {
        // Arrange & Act
        var options = new JobOptions
        {
            IdempotencyKey = "test-key-123"
        };

        // Assert
        options.IdempotencyKey.Should().Be("test-key-123");
    }

    [Fact]
    public void JobOptions_WithRetryPolicy_ShouldStorePolicy()
    {
        // Arrange
        var retryPolicy = new RetryPolicyBuilder()
            .WithMaxAttempts(3)
            .WithExponentialBackoff(TimeSpan.FromSeconds(1), 2.0)
            .Build();

        // Act
        var options = new JobOptions
        {
            RetryPolicy = retryPolicy
        };

        // Assert
        options.RetryPolicy.Should().NotBeNull();
        options.RetryPolicy!.MaxAttempts.Should().Be(3);
    }

    [Fact]
    public void JobOptions_WithPriority_ShouldStorePriority()
    {
        // Arrange & Act
        var options = new JobOptions
        {
            Priority = 10
        };

        // Assert
        options.Priority.Should().Be(10);
    }

    [Fact]
    public void JobOptions_WithTags_ShouldStoreTags()
    {
        // Arrange & Act
        var options = new JobOptions
        {
            Tags = new Dictionary<string, string>
            {
                ["environment"] = "production",
                ["team"] = "backend"
            }
        };

        // Assert
        options.Tags.Should().ContainKey("environment");
        options.Tags!["environment"].Should().Be("production");
    }
}
