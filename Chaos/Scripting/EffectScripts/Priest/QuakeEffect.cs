﻿using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
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
        TargetAnimation = 550
    };

    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(20), false);

    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 55
    };

    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    public override byte Icon => 98;
    public override string Name => "Quake";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (SourceOfEffect is Aisling aisling && (AislingSubject != null))
        {
            if (!aisling.Equals(AislingSubject))
            {
                if (aisling.Group is null || !aisling.Group.Contains(AislingSubject))
                {
                    Subject.Effects.Terminate(Name);
                    SendMessage(AislingSubject, $"{aisling.Name}'s quake effect fades as they are not grouped.", ServerMessageType.ActiveMessage);
                    return;                    
                }
            }
        }

        var targets = GetAffectedTargets();

        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                CalculateDamage(),
                Element.None);

            target.ShowHealth();
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => SendMessage(AislingSubject, "Quake has worn off.", ServerMessageType.OrangeBar1);

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        // Allow only pets and teammates to receive the effect
        if (target is Monster monster && !monster.Script.Is<PetScript>() && !monster.Script.Is<NightmareTeammateScript>())
        {
            SendMessage(source, "Cannot be cast on monsters.", ServerMessageType.OrangeBar1);
            return false;
        }

        if (target.Effects.Contains("Quake") || target.Effects.Contains("Vortex"))
        {
            SendMessage(source, "Target already has Quake or Vortex.", ServerMessageType.OrangeBar1);
            return false;
        }

        target.Animate(Animation);
        SendMessage(source, $"You cast {Name} on {target.Name}.", ServerMessageType.OrangeBar1);
        SendMessage(AislingSubject, $"{source.Name} cast {Name} on you.", ServerMessageType.OrangeBar1);

        return true;
    }

    private IEnumerable<Creature> GetAffectedTargets()
    {
        var options = new AoeShapeOptions
        {
            Source = new Point(Subject.X, Subject.Y),
            Range = 1,
            Direction = Direction.All
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        return Subject.MapInstance
                      .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                      .WithFilter(Subject, TargetFilter.HostileOnly)
                      .WithFilter(Subject, TargetFilter.AliveOnly)
                      .Where(x => !x.MapInstance.IsWall(Subject))
                      .ToList();
    }

    private int CalculateDamage() => (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 6 + 200;

    private void SendMessage(Creature? target, string message, ServerMessageType type)
    {
        if (target is Aisling aisling)
            aisling.Client.SendServerMessage(type, message);
    }
}
