using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Warrior;

public class ExecuteScript : BasicSkillScriptBase
{

    /// <inheritdoc />
    public ExecuteScript(Skill subject)
        : base(subject) { }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            var damage = CalculateDamage(context, target);
            target.ApplyDamage(target, damage);

            int tenPercent = Convert.ToInt32(.1 * target.StatSheet.MaximumHp);
            if (tenPercent >= target.StatSheet.CurrentHp)
            {
                target.ApplyDamage(context.Source, target.StatSheet.CurrentHp);
                int fivePercent = Convert.ToInt32(.05 * context.Source.StatSheet.EffectiveMaximumHp);
                context.Source.ApplyHealing(context.Source, fivePercent);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
                Subject.Cooldown = new TimeSpan(0, 0, 15);
            }
        }
    }

    protected virtual int CalculateDamage(SkillContext context, Creature target)
    {
        int tenPercent = Convert.ToInt32(.1 * context.Source.StatSheet.EffectiveMaximumHp);

        return tenPercent;
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
