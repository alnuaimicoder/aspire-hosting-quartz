using CommunityToolkit.Aspire.Quartz;
using FluentAssertions;

namespace CommunityToolkit.Aspire.Quartz.Abstractions.Tests;

public class RetryPolicyTests
{
    [Fact]
    public void RetryPolicy_WithValidConfiguration_ShouldCreateInstance()
    {
        // Arrange & Act
        var policy = new RetryPolicy
        {
            MaxAttempts = 3,
            InitialDelay = TimeSpan.FromSeconds(1),
            Strategy = BackoffStrategy.Exponential,
            BackoffMultiplier = 2.0,
            MaxDelay = TimeSpan.FromMinutes(5)
        };

        // Assert
        policy.MaxAttempts.Should().Be(3);
        policy.InitialDelay.Should().Be(TimeSpan.FromSeconds(1));
        policy.Strategy.Should().Be(BackoffStrategy.Exponential);
        policy.BackoffMultiplier.Should().Be(2.0);
        policy.MaxDelay.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public void RetryPolicyBuilder_WithExponentialBackoff_ShouldBuildCorrectPolicy()
    {
        // Arrange & Act
        var policy = new RetryPolicyBuilder()
            .WithMaxAttempts(5)
            .WithExponentialBackoff(TimeSpan.FromSeconds(2), 2.0)
            .Build();

        // Assert
        policy.MaxAttempts.Should().Be(5);
        policy.InitialDelay.Should().Be(TimeSpan.FromSeconds(2));
        policy.Strategy.Should().Be(BackoffStrategy.Exponential);
        policy.BackoffMultiplier.Should().Be(2.0);
    }

    [Fact]
    public void RetryPolicyBuilder_WithLinearBackoff_ShouldBuildCorrectPolicy()
    {
        // Arrange & Act
        var policy = new RetryPolicyBuilder()
            .WithMaxAttempts(3)
            .WithLinearBackoff(TimeSpan.FromSeconds(5))
            .Build();

        // Assert
        policy.MaxAttempts.Should().Be(3);
        policy.InitialDelay.Should().Be(TimeSpan.FromSeconds(5));
        policy.Strategy.Should().Be(BackoffStrategy.Linear);
    }
}
