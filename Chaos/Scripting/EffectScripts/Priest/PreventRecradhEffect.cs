using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PreventRecradhEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(14);
    public override byte Icon => 102;
    public override string Name => "preventrecradh";

    public override void OnDispelled() => OnTerminated();
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("preventrecradh"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This magic has already been applied.");

            return false;
        }

        return true;
    }
}