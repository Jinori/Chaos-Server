using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class ClawFistEffect : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(8);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 300,
        TargetAnimation = 54
    };
    private Attributes BonusAttributes = null!;
    
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400), false);

    
    public override byte Icon => 71;
    public override string Name => "Claw Fist";

    public override void OnApplied()
    {
        base.OnApplied();

        BonusAttributes = new Attributes
        {
            SkillDamagePct = GetVar<int>("skillDmgPct"),
            FlatSkillDamage = GetVar<int>("flatSkillDmg")
        };

        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fists rush with a burst of energy.");
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        var pct = source.StatSheet.EffectiveCon / 20;
        var flat = 100 + source.StatSheet.EffectiveCon;

        SetVar("skillDmgPct", pct);
        SetVar("flatSkillDmg", flat);
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed() { }

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hands return to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Claw Fist") || target.Effects.Contains("Chaos Fist"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

            return false;
        }

        return true;
    }
}