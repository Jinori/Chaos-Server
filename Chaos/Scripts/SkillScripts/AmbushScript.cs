using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Chaos.Scripts.SkillScripts;

public class AmbushScript : BasicSkillScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public AmbushScript(Skill subject)
        : base(subject) { }

    protected virtual void ApplyWarp(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        var target = targetEntities.FirstOrDefault();
        if (target != null)
        {
            var s = target.DirectionalOffset(target.Direction.Reverse());
            context.Source.WarpTo(s);
            context.Source.Turn(target.Direction);
            context.Source.SendServerMessage(ServerMessageType.OrangeBar1, "", context.Source.Id);
        }
        else
        {
            context.Source.SendServerMessage(ServerMessageType.OrangeBar1, "There is nothing to gain advantage on.", context.Source.Id);
        }
    }

    /// <inheritdoc />
    public override void OnUse(SkillContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);


        PlaySound(context, affectedPoints);
        ApplyWarp(context, affectedEntities);
    }
}