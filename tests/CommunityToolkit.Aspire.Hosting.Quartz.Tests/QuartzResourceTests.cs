using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.Quartz;
using FluentAssertions;

namespace CommunityToolkit.Aspire.Hosting.Quartz.Tests;

public class QuartzResourceTests
{
    [Fact]
    public void QuartzResource_ShouldHaveCorrectName()
    {
        // Arrange & Act
        var resource = new QuartzResource("test-quartz");

        // Assert
        resource.Name.Should().Be("test-quartz");
    }

    [Fact]
    public void QuartzResource_ShouldImplementIResourceWithConnectionString()
    {
        // Arrange & Act
        var resource = new QuartzResource("test-quartz");

        // Assert
        resource.Should().BeAssignableTo<IResourceWithConnectionString>();
    }

    [Fact]
    public void QuartzResource_ShouldImplementIResourceWithEnvironment()
    {
        // Arrange & Act
        var resource = new QuartzResource("test-quartz");

        // Assert
        resource.Should().BeAssignableTo<IResourceWithEnvironment>();
    }

    [Fact]
    public void QuartzResource_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var resource = new QuartzResource("test-quartz");

        // Assert
        resource.SchedulerName.Should().Be("AspireQuartzScheduler");
        resource.MaxConcurrency.Should().Be(10);
        resource.Provider.Should().Be(DatabaseProvider.SqlServer);
        resource.EnableMigration.Should().BeTrue();
        resource.IdempotencyExpiration.Should().Be(TimeSpan.FromDays(7));
    }
}
