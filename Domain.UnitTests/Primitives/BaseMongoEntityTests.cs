using AutoFixture;
using Common.Tests.Abstractions;
using Domain.Core.Primitives;
using Domain.Entities;

namespace Domain.UnitTests.Primitives;

public sealed class BaseMongoBaseMongoBaseMongoEntityTests
    : BaseUnitTest
{
    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherBaseMongoEntityIsNull()
    {
        // Arrange
        BaseMongoEntity entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherBaseMongoEntityIsTheSameBaseMongoEntity()
    {
        // Arrange
        BaseMongoEntity entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherObjectIsNull()
    {
        // Arrange
        BaseMongoEntity entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals((object)null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherObjectIsTheSameBaseMongoEntity()
    {
        // Arrange
        BaseMongoEntity entity = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity.Equals((object)entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnProperHashCode()
    {
        // Arrange
        BaseMongoEntity entity = Fixture.Create<RabbitMessage>();

        // Act
        int hashcode = entity.GetHashCode();

        // Assert
        hashcode.Should().Be(entity.Id.GetHashCode() * 41);
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenBothEntitiesAreNull()
    {
        // Arrange
        BaseMongoEntity entity1 = null;
        BaseMongoEntity entity2 = null;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenFirstBaseMongoEntityIsNull()
    {
        // Arrange
        BaseMongoEntity entity1 = Fixture.Create<RabbitMessage>();
        BaseMongoEntity entity2 = null;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenSecondBaseMongoEntityIsNull()
    {
        // Arrange
        BaseMongoEntity entity1 = null;
        BaseMongoEntity entity2 = Fixture.Create<RabbitMessage>();

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenEntitiesAreEqual()
    {
        // Arrange
        BaseMongoEntity entity1 = Fixture.Create<RabbitMessage>();
        BaseMongoEntity entity2 = entity1;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }
}