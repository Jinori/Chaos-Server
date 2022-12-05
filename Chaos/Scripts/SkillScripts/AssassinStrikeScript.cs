using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class AssassinStrikeScript : BasicSkillScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    private readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public AssassinStrikeScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) { EffectFactory = effectFactory; }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            if (target.Status.HasFlag(Status.AsgallFaileas))
            {
                //Let's reflect damage back at a 70% chance and take no damage ourselves.
                if (Randomizer.RollChance(70))
                {
                    var reflectDamage = CalculateDamage(context, target);
                    context.Source.ApplyDamage(context.Source, reflectDamage);
                    return;
                }
            }
            var damage = CalculateDamage(context, target);
            target.ApplyDamage(context.Source, damage);
        }
    }

    protected virtual int CalculateDamage(SkillContext context, Creature target)
    {
        //Apply debuff before calculation

        var effect = EffectFactory.Create("AssassinStrike");
        target.Effects.Apply(context.Source, effect);

        var damage = BaseDamage ?? 0;

        if (DamageStat.HasValue)
        {
            var multiplier = DamageStatMultiplier ?? 1;

            damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
        }

        return DamageFormulae.Default.Calculate(context.Source, target, damage);
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