using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class BeagCradhEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Dia Cradh",
            "Ard Cradh",
            "Mor Cradh",
            "Cradh",
            "Beag Cradh"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);

    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 483,
        AnimationSpeed = 100,
        Priority = 100
    };

    public override byte Icon => 4;
    public override string Name => "Beag Cradh";
    protected byte? Sound => 27;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -10
        };

        Subject.Animate(Animation.GetPointAnimation(Subject));
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -10
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "beag cradh curse has been lifted.");
    }

    /// <inheritdoc />
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