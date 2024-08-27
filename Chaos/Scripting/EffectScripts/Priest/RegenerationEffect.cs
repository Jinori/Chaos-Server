using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class RegenerationEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);
    private Creature SourceOfEffect { get; set; } = null!;
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 125
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    protected IApplyHealScript ApplyHealScript { get; }
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 98;
    /// <inheritdoc />
    public override string Name => "Regeneration";

    public RegenerationEffect() => ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();

    public override void OnApplied() =>
        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "Your body starts to regenerate quickly.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        // Calculate the heal amount based on source's WIS and 1% of target's max HP
        var wis = SourceOfEffect.StatSheet.Wis;
        var statmultiplier = wis * 5;
        var targetMaxHp = Subject.StatSheet.EffectiveMaximumHp;
        var healAmount = statmultiplier + (targetMaxHp / 100);  // WIS + 1% of target's max HP

        // Apply the heal to the target
        ApplyHealScript.ApplyHeal(SourceOfEffect, Subject, ApplyHealScript, (int)healAmount);
        Subject.Animate(Animation);
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Regeneration has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        if (target.Effects.Contains("Regeneration"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your ally has Regeneration already.");

            return false;
        }

        return true;
    }
}