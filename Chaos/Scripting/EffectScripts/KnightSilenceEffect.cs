using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class KnightSilenceEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
    public override byte Icon => 87;
    public override string Name => "KnightSilence";

    public override void OnApplied()
    {
        base.OnApplied();

        if (Subject is Aisling aisling)
        {
            aisling.Muted = true;
            aisling.SendServerMessage(ServerMessageType.OrangeBar1, "Your voice is swept away by a strange wind.");
        }
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject is Aisling aisling)
        {
            aisling.Muted = false;
            aisling.SendServerMessage(ServerMessageType.OrangeBar1, "You can talk normally again.");
        }
    }

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target is Aisling aisling)
            if (aisling.IsKnight || aisling.IsAdmin)
                return false;

        return true;
    }
}