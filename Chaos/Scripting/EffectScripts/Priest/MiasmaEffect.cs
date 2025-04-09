using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.EffectScripts.Priest;

public class MiasmaEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 295
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 27;

    /// <inheritdoc />
    public override string Name => "Miasma";
    
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
            0.0075m,
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
        if (target.Script.Is<ThisIsABossScript>())
            return false;

        if (target.IsGodModeEnabled())
            return false;

        if (target.Effects.Contains("Poison"))
        {
            target.Effects.Terminate("Poison");

            return true;
        }

        if (target.Effects.Contains("Miasma"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target has this effect already.");

            return false;
        }

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
        ;
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}