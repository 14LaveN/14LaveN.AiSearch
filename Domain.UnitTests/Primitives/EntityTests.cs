using AutoFixture;
using Common.Tests.Abstractions;
using Domain.UnitTests.TestData.Entity;
using Domain.Common.Core.Primitives;

namespace Domain.UnitTests.Primitives;

public sealed class EntityTests
    : BaseUnitTest
{
    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherEntityIsNull()
    {
        // Arrange
        Entity entity = Fixture.Create<CommonEntity>();

        // Act
        bool result = entity.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherEntityIsTheSameEntity()
    {
        // Arrange
        Entity entity = Fixture.Create<CommonEntity>();

        // Act
        bool result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherObjectIsNull()
    {
        // Arrange
        Entity entity = Fixture.Create<CommonEntity>();

        // Act
        bool result = entity.Equals((object)null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherObjectIsTheSameEntity()
    {
        // Arrange
        Entity entity = Fixture.Create<CommonEntity>();

        // Act
        bool result = entity.Equals((object)entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnProperHashCode()
    {
        // Arrange
        Entity entity = Fixture.Create<CommonEntity>();

        // Act
        int hashcode = entity.GetHashCode();

        // Assert
        hashcode.Should().Be(entity.Id.GetHashCode() * 41);
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenBothEntitiesAreNull()
    {
        // Arrange
        Entity entity1 = null;
        Entity entity2 = null;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenFirstEntityIsNull()
    {
        // Arrange
        Entity entity1 = Fixture.Create<CommonEntity>();
        Entity entity2 = null;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenSecondEntityIsNull()
    {
        // Arrange
        Entity entity1 = null;
        Entity entity2 = Fixture.Create<CommonEntity>();

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenEntitiesAreEqual()
    {
        // Arrange
        Entity entity1 = Fixture.Create<CommonEntity>();
        Entity entity2 = entity1;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }
}