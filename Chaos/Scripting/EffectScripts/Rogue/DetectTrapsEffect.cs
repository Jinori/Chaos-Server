﻿using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class DetectTrapsEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(200);
    public override byte Icon => 84;
    public override string Name => "detectTraps";

    public override void OnApplied()
    {
        base.OnApplied();
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your sense of survival peaks.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your senses have returned to normal.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.StatSheet.CurrentMp < 100)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana.");

            return false;
        }

        if (target.Effects.Contains("detectTraps"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've currently peaked your senses.");

            return false;
        }

        return true;
    }
}