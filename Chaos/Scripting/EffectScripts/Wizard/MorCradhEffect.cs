﻿using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class MorCradhEffect : EffectBase
{
    public override byte Icon => 83;
    public override string Name => "ard cradh";

    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(5);

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 2
        };

        AislingSubject?.StatSheet.SubtractMp(120);
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been cursed by cradh! AC and MR lowered!");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 2
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "mor cradh curse has been lifted.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("ard cradh")
            || target.Effects.Contains("mor cradh")
            || target.Effects.Contains("cradh")
            || target.Effects.Contains("beag cradh"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Another curse has already been applied.");

            return false;
        }

        return true;
    }
}