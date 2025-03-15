using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class FocusEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected Attributes SnapshotAttributes;

    public List<string> ConflictingEffectNames { get; init; } = ["Focus"];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);

    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 88,
        AnimationSpeed = 200,
        Priority = 5
    };

    public override byte Icon => 100;
    public override string Name => "Focus";

    protected byte? Sound => 140;

    public override void OnApplied()
    {
        base.OnApplied();

        var atkSpeedBonus = 20 + Subject.StatSheet.EffectiveDex / 20;
        var skillDamagePctBonus = 10 + Subject.StatSheet.EffectiveDex / 20;
        var flatSkillDamageBonus = 25 + Subject.StatSheet.EffectiveDex;

        SnapshotAttributes = new Attributes
        {
            AtkSpeedPct = atkSpeedBonus,
            SkillDamagePct = skillDamagePctBonus,
            FlatSkillDamage = flatSkillDamageBonus
        };

        Subject.StatSheet.AddBonus(SnapshotAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are now focused.");
        Subject.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(SnapshotAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You lost your focus.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}