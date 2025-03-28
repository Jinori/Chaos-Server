#region
using Chaos.Common.Identity;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class PersistentIdGeneratorTests : IDisposable
{
    private const string FILE_PATH = $"PersistentId{nameof(Int32)}.json";

    public PersistentIdGeneratorTests()
    {
        if (File.Exists(FILE_PATH))
            File.Delete(FILE_PATH);
    }

    public void Dispose()
    {
        if (File.Exists(FILE_PATH))
            File.Delete(FILE_PATH);
    }

    [Test]
    public void NextId_ShouldGenerateNextSequentialId()
    {
        // Arrange
        var generator = new PersistentIdGenerator<int>("Test", 5);

        // Act
        var id1 = generator.NextId;

        // Assert
        id1.Should()
           .Be(6);
    }

    [Test]
    public void NextId_ShouldGenerateSequentialIds()
    {
        // Arrange
        var idGenerator = PersistentIdGenerator<int>.Shared;

        // Act
        var id1 = idGenerator.NextId;
        var id2 = idGenerator.NextId;

        // Assert
        id2.Should()
           .BeGreaterThan(id1);
    }

    [Test]
    public void NextId_ShouldPersistIdBetweenInstances()
    {
        // Arrange
        var id1 = PersistentIdGenerator<int>.Shared.NextId;

        // Act
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var id2 = PersistentIdGenerator<int>.Shared.NextId;

        // Assert
        id2.Should()
           .BeGreaterThan(id1);
    }
}