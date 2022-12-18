using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Warrior;

public class CrasherScript : BasicSkillScriptBase
{

    /// <inheritdoc />
    public CrasherScript(Skill subject)
        : base(subject) { }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            var damage = CalculateDamage(context, target);
            target.ApplyDamage(context.Source, damage);
        }

        int sac = Convert.ToInt32(.8 * context.Source.StatSheet.CurrentHp);
        if (context.Source.StatSheet.CurrentHp <= sac)
        {
            context.Source.StatSheet.SetHp(1);
        }
        else
            context.Source.StatSheet.SubtractHp(sac);
        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    protected virtual int CalculateDamage(SkillContext context, Creature target)
    {
        int damage = Convert.ToInt32(1.6 * context.Source.StatSheet.CurrentHp);

        return damage;
    }

    /// <inheritdoc />
    protected override IEnumerable<T> GetAffectedEntities<T>(SkillContext context, IEnumerable<IPoint> affectedPoints)
    {
        var entities = base.GetAffectedEntities<T>(context, affectedPoints);

        return entities;
    }

    /// <inheritdoc />
    public override void OnUse(SkillContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);
    }
}
