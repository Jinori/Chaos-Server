#region
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class DeepCloneTests
{
    [Test]
    public void Create_CreatesDeepClone()
    {
        // Arrange
        var testObj = new MockClonable
        {
            IntValue = 1,
            StringValue = "Test",
            IntList =
            [
                1,
                2,
                3
            ],
            NestedClass = new MockClonable
            {
                IntValue = 2
            }
        };

        // Act
        var clone = DeepClone.Create(testObj)!;

        // Assert
        clone.Should()
             .NotBeNull();

        clone.Should()
             .NotBeSameAs(testObj);

        clone.IntValue
             .Should()
             .Be(testObj.IntValue);

        clone.StringValue
             .Should()
             .Be(testObj.StringValue);

        clone.IntList
             .Should()
             .NotBeSameAs(testObj.IntList);

        clone.IntList
             .Should()
             .Equal(testObj.IntList);

        clone.NestedClass
             .Should()
             .NotBeSameAs(testObj.NestedClass);

        clone.NestedClass
             .IntValue
             .Should()
             .Be(testObj.NestedClass.IntValue);
    }

    [Test]
    public void Create_CreatesDeepCloneOfArray()
    {
        // Arrange
        var testArray = new[]
        {
            1,
            2,
            3
        };

        // Act
        var clone = DeepClone.Create(testArray);

        // Assert
        clone.Should()
             .NotBeNull();

        clone.Should()
             .NotBeSameAs(testArray);

        clone.Should()
             .Equal(testArray);
    }

    [Test]
    public void Create_CreatesDeepCloneOfArrayOfTestClass()
    {
        // Arrange
        var testArray = new[]
        {
            new MockClonable
            {
                IntValue = 1,
                StringValue = "Test1",
                IntList =
                [
                    1,
                    2,
                    3
                ],
                NestedClass = new MockClonable
                {
                    IntValue = 2
                }
            },
            new MockClonable
            {
                IntValue = 2,
                StringValue = "Test2",
                IntList =
                [
                    4,
                    5,
                    6
                ],
                NestedClass = new MockClonable
                {
                    IntValue = 3
                }
            }
        };

        // Act
        var clone = DeepClone.Create(testArray)!;

        // Assert
        clone.Should()
             .NotBeNull();

        clone.Should()
             .NotBeSameAs(testArray);

        clone.Length
             .Should()
             .Be(testArray.Length);

        for (var i = 0; i < clone.Length; i++)
        {
            var clonedObject = clone[i];
            var originalObject = testArray[i];

            clonedObject.Should()
                        .NotBeSameAs(originalObject);

            clonedObject.IntValue
                        .Should()
                        .Be(originalObject.IntValue);

            clonedObject.StringValue
                        .Should()
                        .Be(originalObject.StringValue);

            clonedObject.IntList
                        .Should()
                        .NotBeSameAs(originalObject.IntList);

            clonedObject.IntList
                        .Should()
                        .Equal(originalObject.IntList);

            clonedObject.NestedClass
                        .Should()
                        .NotBeSameAs(originalObject.NestedClass);

            clonedObject.NestedClass
                        .IntValue
                        .Should()
                        .Be(originalObject.NestedClass.IntValue);
        }
    }

    [Test]
    public void Create_ReturnsNullForNullInput()
    {
        // Act
        var clone = DeepClone.Create<MockClonable>(null!);

        // Assert
        clone.Should()
             .BeNull();
    }

    internal sealed class MockClonable
    {
        public List<int> IntList { get; set; } = null!;
        public int IntValue { get; set; }
        public MockClonable NestedClass { get; set; } = null!;
        public string StringValue { get; set; } = null!;
    }
}