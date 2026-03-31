using CommunityToolkit.Aspire.Quartz;
using FluentAssertions;

namespace CommunityToolkit.Aspire.Quartz.Tests;

public class DuplicateJobExceptionTests
{
    [Fact]
    public void DuplicateJobException_ShouldHaveCorrectMessage()
    {
        // Arrange
        var idempotencyKey = "test-key-123";

        // Act
        var exception = new DuplicateJobException(idempotencyKey);

        // Assert
        exception.Message.Should().Contain(idempotencyKey);
        exception.IdempotencyKey.Should().Be(idempotencyKey);
    }
}
