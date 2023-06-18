using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PreventRecradhEffect : EffectBase
{
    public override byte Icon => 112;
    public override string Name => "preventrecradh";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(14);

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.PreventRecradh))
            Subject.Status = Status.PreventRecradh;
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.PreventRecradh))
            Subject.Status &= ~Status.PreventRecradh;
    }

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