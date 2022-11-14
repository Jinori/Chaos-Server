using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class SpellTemplateCache : SimpleFileCacheBase<SpellTemplate, SpellTemplateSchema, SpellTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<SpellTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public SpellTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SpellTemplateCacheOptions> options,
        ILogger<SpellTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}