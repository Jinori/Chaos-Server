using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class ChargeScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public ChargeScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        //get the 5 points in front of the source
        var points = AoeShape.Front.ResolvePoints(
            context.SourcePoint,
            5,
            context.Source.Direction,
            context.Map.Template.Bounds);

        var targets = AbilityComponent.Activate<Creature>(context, this);
        var targetCreature = targets.TargetEntities.FirstOrDefault();

        if (targetCreature is null)
        {
            //if that point is not walkable, continue
            if (!context.Map.IsWalkable(points.Last(), context.Source.Type))
                return;

            //if it is walkable, warp to that point
            context.Source.WarpTo(points.Last());
            DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
        }
    }
}