using Chaos.Collections;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Lava_Flow;

public sealed class LavaFlowFFAHostNotPlayingScript : ArenaScriptBase
{
    /// <inheritdoc />
    public LavaFlowFFAHostNotPlayingScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject, simpleCache) { }

    /// <inheritdoc />
    public override List<string> MorphTemplateKeys { get; set; } = new()
        { "26007", "26008", "26009" };

    /// <inheritdoc />
    public override string MorphOriginalTemplateKey { get; set; } = "26006";
    /// <inheritdoc />
    public override bool TeamGame { get; set; } = false;
    /// <inheritdoc />
    public override bool IsHostPlaying { get; set; } = false;
    /// <inheritdoc />
    public override bool ShouldMapShrink { get; set; } = true;
}