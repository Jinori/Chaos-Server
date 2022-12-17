using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class ThrowSurigumScript : BasicSkillScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public ThrowSurigumScript(Skill subject)
        : base(subject) { }

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
        var damage = BaseDamage ?? 0;

        if (context.Source.Status.HasFlag(Status.ClawFist) && Subject.Template.IsAssail)
        {
            damage = Convert.ToInt32(BaseDamage * 1.3);
        }

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
        if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Slot is not null)
        {
            if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey.ContainsI("dagger") is false)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "In an attempt to throw your weapon, you have failed.");
                return;
            }
        }

        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);
    }
}