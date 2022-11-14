using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class LootTableCache : SimpleFileCacheBase<LootTable, LootTableSchema, LootTableCacheOptions>
{
    /// <inheritdoc />
    protected override Func<LootTable, string> KeySelector => l => l.Key;

    /// <inheritdoc />
    public LootTableCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<LootTableCacheOptions> options,
        ILogger<LootTableCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) =>
        AsyncHelpers.RunSync(ReloadAsync);
}