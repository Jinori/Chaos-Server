using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class BurnEffect : ContinuousAnimationEffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Burn",
            "Firestorm",
            "Fire Punch",
            "Small Firestorm"
        ];

    protected virtual decimal AislingBurnPercentage { get; } = 2.5m;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 60,
        Priority = 15
    };

    /// <inheritdoc />
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    protected virtual decimal BurnPercentage { get; } = 5m;

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override sealed byte Icon => 31;

    /// <inheritdoc />
    public override string Name => "Burn";

    public override void OnApplied() => AislingSubject?.SendOrangeBarMessage("Your body catches fire.");

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are no longer burning.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Name == "Shamensyth")
            return false;

        if (target.StatSheet.DefenseElement == Element.Fire)
            return false;

        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }

    protected virtual int EstimateDamage()
    {
        //5  = 1,500
        //15 = 4,500
        //30 = 9,000
        //50 = 15,000
        //75 = 22,500
        //99 = 29,700

        //Scaling cap based on level and estimate of normal monster hp
        var coefficient = Subject.StatSheet.Level > 99 ? 600 : 300;
        var estimatedHp = Subject.StatSheet.Level * coefficient;
        var estimatedBurn = MathEx.GetPercentOf<int>(estimatedHp, BurnPercentage);

        if (Subject is Aisling)
        {
            var potentialBurn = MathEx.GetPercentOf<int>((int)Subject.StatSheet.EffectiveMaximumHp, AislingBurnPercentage);
            var damage = Math.Min(estimatedBurn, potentialBurn);

            return damage;
        } else
        {
            var potentialBurn = MathEx.GetPercentOf<int>((int)Subject.StatSheet.EffectiveMaximumHp, BurnPercentage);
            var damage = Math.Min(estimatedBurn, potentialBurn);

            return damage;
        }
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var damagePerTick = EstimateDamage();

        if (Subject.IsGodModeEnabled() || Subject.Effects.Contains("invulnerability"))
        {
            Subject.Effects.Terminate("burn");

            return;
        }

        if (Subject.StatSheet.CurrentHp <= damagePerTick)
            return;

        if (Subject.StatSheet.TrySubtractHp(damagePerTick))
        {
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.ShowHealth();
        }
    }
}