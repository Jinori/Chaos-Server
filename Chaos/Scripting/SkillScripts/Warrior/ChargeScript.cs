﻿using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class ChargeScript : ConfigurableSkillScriptBase,
                            GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                            DamageAbilityComponent.IDamageComponentOptions,
                            ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public decimal? DamageMultiplierPerTarget { get; init; }

    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    public int? EffectApplyChance { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />

    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    public decimal PctManaCost { get; init; }

    public decimal? PctOfHealth { get; init; }

    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    public bool? SurroundingTargets { get; init; }

    /// <inheritdoc />
    public ChargeScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        // Get the endpoint 4 tiles away from the source in the same direction
        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, 4);

        // Get all points between the source and the endpoint, skipping the first one
        var points = context.Source
                            .GetDirectPath(endPoint)
                            .Skip(1);

        // Iterate through each point
        foreach (var point in points)
        {
            // If there is a wall at this point, return
            if (context.SourceMap.IsWall(point) || context.SourceMap.IsBlockingReactor(point))
            {
                var distance = point.ManhattanDistanceFrom(context.Source);

                if (distance == 1)
                    return;

                var newPoint = point.OffsetTowards(context.Source);
                context.Source.WarpTo(newPoint);

                return;
            }

            // Get the closest creature to the source at this point
            var entity = context.SourceMap
                                .GetEntitiesAtPoints<Creature>(point)
                                .OrderBy(creature => creature.ManhattanDistanceFrom(context.Source))
                                .FirstOrDefault(creature => context.Source.WillCollideWith(creature));

            // If there is a creature at this point
            if (entity != null)
            {
                // Get the point towards the source from the creature
                var newPoint = entity.OffsetTowards(context.Source);

                // Warp the source to the new point
                context.Source.WarpTo(newPoint);

                // Needs Damage & Ability Component or a new Movement Component
                new ComponentExecutor(context).WithOptions(this)
                                              .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                              ?.Execute<DamageAbilityComponent>()
                                              .Execute<ApplyEffectAbilityComponent>();

                return;
            }
        }

        // If no creature was found, warp the source to the endpoint
        context.Source.WarpTo(endPoint);
    }
}