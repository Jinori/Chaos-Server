using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class ShadowFigureScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    /// <inheritdoc />
    public ShadowFigureScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();

        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageMultiplier = DamageMultiplier,
            DamageStat = DamageStat
        };
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        //get the first point in front of the source
        var points = AoeShape.Front.ResolvePoints(
            context.SourcePoint,
            1,
            context.Source.Direction,
            context.Map.Template.Bounds);

        //get the first creature along those points
        var targetCreature = context.Map.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
            .FirstOrDefault();

        //if there is no creature, return
        if (targetCreature == null)
            return;

        //if the target is standing on a wall, return
        if (context.Map.IsWall(targetCreature))
            return;

        //if any point on the path to the creature is a wall, return
        if (context.Source.GetDirectPath(targetCreature).Any(pt => context.Map.IsWall(pt)))
            return;

        //get the direction that vectors behind the target relative to the source
        var behindTargetDirection = targetCreature.DirectionalRelationTo(context.SourcePoint);

        //for each direction around the target, starting with the direction behind the target
        foreach (var direction in behindTargetDirection.AsEnumerable())
        {
            //get the point in that direction
            var destinationPoint = targetCreature.DirectionalOffset(direction);
            //if that point is not walkable, continue
            if (!context.Map.IsWalkable(destinationPoint, context.Source.Type))
                continue;

            //if it is walkable, warp to that point and turn to face the target
            context.Source.WarpTo(destinationPoint);
            var newDirection = targetCreature.DirectionalRelationTo(context.Source);
            context.Source.Turn(newDirection);
            var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
            DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
            return;
        }
    }

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageMultiplier { get; init; }
    #endregion
}