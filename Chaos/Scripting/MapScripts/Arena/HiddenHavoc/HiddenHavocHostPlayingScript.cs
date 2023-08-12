using Chaos.Collections;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.HiddenHavoc;

public sealed class HiddenHavocHostPlayingScript : HiddenHavocGameScript
{
    /// <inheritdoc />
    public HiddenHavocHostPlayingScript(MapInstance subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
        : base(subject, simpleCache, effectFactory) { }

    /// <inheritdoc />
    public override List<string> MorphTemplateKeys { get; set; } = new()
        { "26007", "26008", "26009", "26010", "26011"};

    /// <inheritdoc />
    public override string MorphOriginalTemplateKey { get; set; } = "26006";
    
    /// <inheritdoc />
    public override bool IsHostPlaying { get; set; } = true;
    /// <inheritdoc />
    public override bool ShouldMapShrink { get; set; } = true;
}