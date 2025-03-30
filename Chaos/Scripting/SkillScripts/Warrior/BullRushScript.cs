using Chaos.DarkAges.Definitions;
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

public class BullRushScript : ConfigurableSkillScriptBase,
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

    public int? HealthCost { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    public decimal PctHealthCost { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
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
    public BullRushScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        // Get the endpoint 4 tiles away from the source in the same direction
        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, 2);

        // Get all points between the source and the endpoint, skipping the first one
        var points = context.Source
                            .GetDirectPath(endPoint)
                            .Skip(1)
                            .ToArray();

        var newPoint = context.SourcePoint;

        // Iterate through each point
        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];

            //if the point we're looking at is not walkable
            if (!context.SourceMap.IsWalkable(point, context.Source.Type, false))
            {
                //if it's the first point, just break
                //we're already standing on the point to charge to
                if (i == 0)
                    break;

                //otherwise set the new point to the previous point
                newPoint = points[i - 1];

                break;
            }

            //if we reach here on the last point (it's walkable)
            //set the new point to the last point
            if (i == (points.Length - 1))
                newPoint = points.Last();
        }

        //if the new point is not the same as the source point
        if (newPoint != context.SourcePoint)
        {
            //warp the source to the new point and update the context
            context.Source.WarpTo(newPoint);
            context = context.WithUpdatedOrigin();
        }

        //deal damage according to selected options
        new ComponentExecutor(context).WithOptions(this)
                                      .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                      ?.Execute<DamageAbilityComponent>()
                                      .Execute<ApplyEffectAbilityComponent>();
    }
}