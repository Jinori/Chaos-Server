using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class ChargeScript : ConfigurableSkillScriptBase,
                            AbilityComponent<Creature>.IAbilityComponentOptions,
                            DamageComponent.IDamageComponentOptions
{
    
    /// <inheritdoc />
    public ChargeScript(Skill subject)
        : base(subject)
    {
        SourceScript = this;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        // Get the endpoint 4 tiles away from the source in the same direction
        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, 4);
        // Get all points between the source and the endpoint, skipping the first one
        var points = context.Source.GetDirectPath(endPoint).Skip(1);
        // Iterate through each point
        foreach (var point in points)
        {
            // If there is a wall at this point, return
            if (context.SourceMap.IsWall(point) || context.SourceMap.IsBlockingReactor(point))
            {
                var distance = point.DistanceFrom(context.Source);
                if (distance == 1)
                    return;

                var newPoint = point.OffsetTowards(context.Source);
                context.Source.WarpTo(newPoint);
                return;
            }
            // Get the closest creature to the source at this point
            var entity = context.SourceMap.GetEntitiesAtPoint<Creature>(point).OrderBy(creature => creature.DistanceFrom(context.Source))
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
                                              .ExecuteAndCheck<AbilityComponent<Creature>>()
                                              ?.Execute<DamageComponent>();
                
                return;
            }
        }

        // If no creature was found, warp the source to the endpoint
        context.Source.WarpTo(endPoint);
    }
    
    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }
    /// <inheritdoc />
    public int? BaseDamage { get; init; }
    /// <inheritdoc />
    public Stat? DamageStat { get; init; }
    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }
    /// <inheritdoc />
    public Element? Element { get; init; }
    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }
    /// <inheritdoc />
    public IScript SourceScript { get; init; }
    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
}