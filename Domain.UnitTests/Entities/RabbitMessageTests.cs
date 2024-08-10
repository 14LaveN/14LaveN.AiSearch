using AutoFixture;
using Common.Tests.Abstractions;
using Domain.Entities;

namespace Domain.UnitTests.Entities;

public sealed class RabbitMessageTests
    : BaseUnitTest
{
    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherRabbitMessageIsNull()
    {
        // Arrange
        RabbitMessage entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals(null);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void Equals_ShouldReturnNewEntity_WhenParametersEqualsEntity()
    {
        // Arrange
        string description = Fixture.Create<string>();

        // Act
        RabbitMessage entity = description;

        // Assert
        entity.Description.Should().Be(description);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherRabbitMessageIsTheSameRabbitMessage()
    {
        // Arrange
        RabbitMessage entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }
}