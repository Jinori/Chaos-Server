﻿using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PreventRecradhEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(8);
    public override byte Icon => 102;
    public override string Name => "Prevent Recradh";

    public override void OnDispelled() => OnTerminated();

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Prevent Recradh"))
            return false;
        
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You resist cradhs for a short while.");
        return true;
    }
}