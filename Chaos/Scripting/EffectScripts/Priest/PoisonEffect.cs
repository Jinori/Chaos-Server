using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;
using NLog.Targets;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PoisonEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 247,
        Priority = 70
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 27;

    /// <inheritdoc />
    public override string Name => "Poison";

    private readonly IApplyDamageScript ApplyDamageScript = ApplyNonAttackDamageScript.Create();

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.IsGodModeEnabled() || Subject.Effects.Contains("Invulnerability"))
        {
            Subject.Effects.Terminate("Poison");

            return;
        }
        
        var damage = DamageHelper.CalculatePercentDamage(
            Source,
            Subject,
            0.5m,
            true);

        ApplyDamageScript.ApplyDamage(
            Source,
            Subject,
            this,
            damage);

        Subject.Animate(Animation);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.IsGodModeEnabled())
            return false;

        if (target.Effects.Contains("Poison Immunity"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is currently immune to poison.");

            return false;
        }

        if (target.Effects.Contains("Poison"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target has this effect already.");

            return false;
        }

        if (target.Effects.Contains("Miasma"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is currently affected by Miasma.");

            return false;
        }

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
        ;
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}