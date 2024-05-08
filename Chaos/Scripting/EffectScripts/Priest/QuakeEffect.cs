using Chaos.Collections;
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

public class QuakeEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);
    private Creature SourceOfEffect { get; set; } = null!;
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 55
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(20000));
    protected IApplyDamageScript ApplyDamageScript { get; }

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 55
    };
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    public override byte Icon => 98;
    /// <inheritdoc />
    public override string Name => "Quake";

    public QuakeEffect() => ApplyDamageScript = ApplyAttackDamageScript.Create();

    public override void OnApplied() =>
        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "A quake has started around you.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var points = AoeShape.AllAround.ResolvePoints(Subject);

        var targets =
            Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly).ToList();

        if ((AislingSubject?.Group == null && AislingSubject?.Name != SourceOfEffect.Name && !Subject.Script.Is<PetScript>() && !Subject.Script.Is<NightmareTeammateScript>())
            || AislingSubject?.Group != null && !AislingSubject.Group.Contains(SourceOfEffect) && !Subject.Script.Is<PetScript>() && !Subject.Script.Is<NightmareTeammateScript>())
            Subject.Effects.Terminate("Quake");
        
        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                Subject,
                target,
                this,
                (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 6 + 200,
                Element.None);

            target.ShowHealth();
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Quake has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;
        
        if (!target.IsFriendlyTo(source))
        {
            if (target.Name.Contains("Teammate") || target.Script.Is<PetScript>())
                return true;
            
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is not an ally.");

            return false;
        }

        if (target.Effects.Contains("quake"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target has already has Quake.");

            return false;
        }

        return true;
    }
}