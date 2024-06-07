using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class ArdCradhEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(3);
    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 486,
        AnimationSpeed = 100
    };
    public List<string> ConflictingEffectNames { get; init; } =
    [
        "ard cradh",
        "mor cradh",
        "cradh",
        "beag cradh"
    ];
    public override byte Icon => 63;
    public override string Name => "ard cradh";
    protected byte? Sound => 27;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 30
        };

        Subject.Animate(Animation);
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've been cursed by ard cradh! AC and MR lowered!");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 30
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Ard Cradh curse has been lifted.");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.IsCradhLocked())
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are prevented from afflicting curses.");

            return false;
        }

        if (target.IsCradhPrevented())
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target cannot be cursed at this time.");

            return false;
        }

        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}