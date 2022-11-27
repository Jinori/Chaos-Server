using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class WorldMapNodeCache : SimpleFileCacheBase<WorldMapNode, WorldMapNodeSchema, WorldMapNodeCacheOptions>
{
    /// <inheritdoc />
    protected override Func<WorldMapNode, string> KeySelector { get; } = n => n.NodeKey;

    /// <inheritdoc />
    public WorldMapNodeCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<WorldMapNodeCacheOptions> options,
        ILogger<WorldMapNodeCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}