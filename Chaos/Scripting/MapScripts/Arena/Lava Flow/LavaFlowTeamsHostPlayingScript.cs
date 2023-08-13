using Chaos.Collections;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Lava_Flow;

public sealed class LavaFlowTeamsHostPlayingScript : ArenaScriptBase
{
    /// <inheritdoc />
    public override bool IsHostPlaying { get; set; } = true;

    /// <inheritdoc />
    public override string MorphOriginalTemplateKey { get; set; } = "26006";

    /// <inheritdoc />
    public override List<string> MorphTemplateKeys { get; set; } = new()
        { "26007", "26008", "26009" };
    /// <inheritdoc />
    public override bool ShouldMapShrink { get; set; } = true;
    /// <inheritdoc />
    public override bool TeamGame { get; set; } = true;

    /// <inheritdoc />
    public LavaFlowTeamsHostPlayingScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject, simpleCache) { }
}