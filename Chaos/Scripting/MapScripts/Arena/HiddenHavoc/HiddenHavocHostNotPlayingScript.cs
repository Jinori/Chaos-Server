using Chaos.Collections;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.HiddenHavoc;

public sealed class HiddenHavocHostNotPlayingScript : HiddenHavocGameScript
{
    public override bool IsHostPlaying { get; set; } = false;

    /// <inheritdoc />
    public override string MorphOriginalTemplateKey { get; set; } = "26006";

    /// <inheritdoc />
    public override List<string> MorphTemplateKeys { get; set; } = new()
        { "26007", "26008", "26009", "26010", "26011" };

    /// <inheritdoc />
    public override bool ShouldMapShrink { get; set; } = true;

    /// <inheritdoc />
    public HiddenHavocHostNotPlayingScript(MapInstance subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
        : base(subject, simpleCache, effectFactory) { }
}