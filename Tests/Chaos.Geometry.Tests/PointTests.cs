#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class PointTests
{
    [Test]
    public void Point_Constructor_CreatesPointWithGivenCoordinates()
    {
        // Arrange
        const int X = 10;
        const int Y = 20;

        // Act
        var point = new Point(X, Y);

        // Assert
        point.X
             .Should()
             .Be(X);

        point.Y
             .Should()
             .Be(Y);
    }

    [Test]
    public void Point_Deconstructor_CreatesPointWithGivenCoordinates()
    {
        // Arrange
        const int X = 10;
        const int Y = 20;

        // Act
        (var x, var y) = new Point(X, Y);

        // Assert
        x.Should()
         .Be(X);

        y.Should()
         .Be(Y);
    }

    [Test]
    public void Point_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var point = new Point(10, 20);
        var otherObject = new object();

        // Act
        var result = point.Equals(otherObject);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Point_Equals_ReturnsFalseWhenPointsAreNotEqual()
    {
        // Arrange
        var point1 = new Point(10, 20);
        var point2 = new Point(30, 40);

        // Act
        var result = point1.Equals(point2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Point_Equals_ReturnsTrueWhenPointsAreEqual()
    {
        // Arrange
        const int X = 10;
        const int Y = 20;
        var point1 = new Point(X, Y);
        var point2 = new Point(X, Y);

        // Act
        var result = point1.Equals(point2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Point_From_ReturnsNewPointWithSameValuesWhenPassedPointOfDifferentType()
    {
        // Arrange
        IPoint originalPoint = new MockPoint(10, 20);

        // Act
        var newPoint = Point.From(originalPoint);

        // Assert
        newPoint.X
                .Should()
                .Be(originalPoint.X);

        newPoint.Y
                .Should()
                .Be(originalPoint.Y);
    }

    [Test]
    public void Point_From_ReturnsSamePointWhenPassedPointOfSameType()
    {
        // Arrange
        var originalPoint = new Point(10, 20);

        // Act
        var newPoint = Point.From(originalPoint);

        // Assert
        newPoint.Should()
                .BeEquivalentTo(originalPoint);
    }

    [Test]
    public void Point_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var point = new Point(10, 20);
        const int EXPECTED_HASH_CODE = (10 << 16) + 20;

        // Act
        var hashCode1 = point.GetHashCode();
        var hashCode2 = point.GetHashCode();

        // Assert
        hashCode1.Should()
                 .Be(EXPECTED_HASH_CODE);

        hashCode2.Should()
                 .Be(EXPECTED_HASH_CODE);
    }

    [Test]
    public void Point_TryParse_InvalidInput_ReturnsFalseAndDefaultPoint()
    {
        // Arrange
        const string INPUT = "Invalid input";

        // Act
        var result = Point.TryParse(INPUT, out var point);

        // Assert
        result.Should()
              .BeFalse();

        point.Should()
             .Be(default(Point));
    }

    [Test]
    public void Point_TryParse_ValidInput_ReturnsTrueAndParsesPoint()
    {
        // Arrange
        const string INPUT = "(123, 456)";
        const int EXPECTED_X = 123;
        const int EXPECTED_Y = 456;

        // Act
        var result = Point.TryParse(INPUT, out var point);

        // Assert
        result.Should()
              .BeTrue();

        point.X
             .Should()
             .Be(EXPECTED_X);

        point.Y
             .Should()
             .Be(EXPECTED_Y);
    }

    // CustomPoint class for testing Point.From method
    private sealed class MockPoint(int x, int y) : IPoint
    {
        public int X { get; } = x;
        public int Y { get; } = y;
    }
}