using AutoFixture;
using Common.Tests.Abstractions;
using Domain.Entities;

namespace Domain.UnitTests.Entities;

public class MetricsEntityTests
    : BaseUnitTest
{
    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherMetricEntityIsNull()
    {
        // Arrange
        MetricEntity entity = Fixture.Create<MetricEntity>();

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
        string name = Fixture.Create<string>();

        // Act
        MetricEntity entity = MetricEntity.ToMetricEntity(description, name);

        // Assert
        entity.Description.Should().Be(description);
        entity.Name.Should().Be(name);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenOtherMetricEntityIsTheSameMetricEntity()
    {
        // Arrange
        MetricEntity entity = Fixture.Create<MetricEntity>();

        // Act
        bool result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }
}