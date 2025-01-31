﻿using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class TideStanceEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    public override byte Icon => 91;
    public override string Name => "Tide Stance";

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A heavy, soothing mist envelops your body.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The mist rolls off your body.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Mist Stance") && !target.Effects.Contains("Tide Stance"))
            return true;

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

        return false;
    }
}