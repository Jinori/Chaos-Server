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

public class VortexEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    private Creature? SourceOfEffect { get; set; }

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 587
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(20000), false);

    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 283
    };

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 98;

    /// <inheritdoc />
    public override string Name => "Vortex";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (SourceOfEffect == null)
        {
            AislingSubject?.Effects.Terminate(Name);

            return;
        }

        if ((AislingSubject == null) && !Subject.Script.Is<PetScript>())
        {
            Subject.Effects.Terminate("Vortex");

            return;
        }

        if (Subject.MapInstance != SourceOfEffect.MapInstance)
        {
            Subject.Effects.Terminate("Vortex");

            return;
        }

        var options = new AoeShapeOptions
        {
            Source = new Point(Subject.X, Subject.Y),
            Range = 1,
            Direction = Direction.All
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        var targets = Subject.MapInstance
                             .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                             .WithFilter(Subject, TargetFilter.HostileOnly)
                             .WithFilter(Subject, TargetFilter.AliveOnly)
                             .Where(x => !x.Equals(Subject) && !x.MapInstance.IsWall(Subject))
                             .ToList();

        if (((AislingSubject?.Group == null)
             && (AislingSubject?.Name != SourceOfEffect.Name)
             && !Subject.Script.Is<PetScript>()
             && !Subject.Script.Is<NightmareTeammateScript>())
            || ((AislingSubject?.Group != null)
                && !AislingSubject.Group.Contains(SourceOfEffect)
                && !Subject.Script.Is<PetScript>()
                && !Subject.Script.Is<NightmareTeammateScript>()))
            Subject.Effects.Terminate("Vortex");

        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 24 + 500,
                Element.None);

            target.ShowHealth();
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Vortex has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        if (!target.IsFriendlyTo(source))
        {
            if (target.Name.Contains("Teammate") || target.Script.Is<PetScript>())
            {
                Subject.Animate(Animation);

                return true;
            }

            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is not an ally.");

            return false;
        }

        if (target.Effects.Contains("Quake"))
        {
            target.Effects.Terminate("Quake");
            Subject.Animate(Animation);
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

            return true;
        }

        if (target.Effects.Contains("Vortex"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target has already has Vortex.");

            return false;
        }

        Subject.Animate(Animation);
        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}