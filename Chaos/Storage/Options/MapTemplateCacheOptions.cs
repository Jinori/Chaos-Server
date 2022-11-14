using Chaos.Storage.Abstractions;

namespace Chaos.Storage.Options;

public sealed class MapTemplateCacheOptions : FileCacheOptionsBase
{
    public string MapDataDirectory { get; set; } = null!;

    /// <inheritdoc />
    public override void UseBaseDirectory(string rootDirectory)
    {
        base.UseBaseDirectory(rootDirectory);
        MapDataDirectory = Path.Combine(rootDirectory, MapDataDirectory);
    }
}