﻿using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class EarthenStanceEffect : EffectBase
{
    public override byte Icon => 54;
    public override string Name => "earthenstance";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.EarthenStance))
            Subject.Status = Status.EarthenStance;
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Gaea protects your body, surrounding you in stone.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.EarthenStance))
            Subject.Status &= ~Status.EarthenStance;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The pebbles and rock flow from your body.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.StatSheet.CurrentMp < 200)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana.");

            return false;
        }

        if (target.Effects.Contains("earthenstance"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

            return false;
        }

        return true;
    }
}