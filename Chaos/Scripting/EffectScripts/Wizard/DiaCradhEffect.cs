using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class DiaCradhEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 257,
        AnimationSpeed = 100
    };
    public List<string> ConflictingEffectNames { get; init; } =
    [
        "dia cradh",
        "ard cradh",
        "mor cradh",
        "cradh",
        "beag cradh"
    ];
    public override byte Icon => 78;
    public override string Name => "dia cradh";
    protected byte? Sound => 27;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -65,
            MagicResistance = 40
        };

        Subject.Animate(Animation.GetPointAnimation(Subject));
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've been cursed by dia cradh! AC and MR lowered!");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -65,
            MagicResistance = 40
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Dia Cradh curse has been lifted.");
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