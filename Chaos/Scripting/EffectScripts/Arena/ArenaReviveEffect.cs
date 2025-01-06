using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Arena;

public class ArenaReviveEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    public override byte Icon => 109;
    public override string Name => "Arena Revive";

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can now enter the Battle Ring.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Arena Revive"))
            return true;

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You're already waiting to enter the Arena.");

        return false;
    }
}