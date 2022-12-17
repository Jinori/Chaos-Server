using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Monk;

public class PoisonPunchScript : BasicSkillScriptBase
{
    private readonly IEffectFactory EffectFactory;
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public PoisonPunchScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) { EffectFactory = effectFactory; }

    protected virtual void ApplyDamage(SkillContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            var damage = CalculateDamage(context, target);
            target.ApplyDamage(context.Source, damage);
            var effect = EffectFactory.Create("Poison");
            target.Effects.Apply(context.Source, effect);
        }
    }

    protected virtual int CalculateDamage(SkillContext context, Creature target)
    {
        var damage = BaseDamage ?? 0;

        if (DamageStat.HasValue)
        {
            var multiplier = DamageStatMultiplier ?? 1;

            damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
        }

        return DamageFormulae.Default.Calculate(context.Source, target, damage);
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