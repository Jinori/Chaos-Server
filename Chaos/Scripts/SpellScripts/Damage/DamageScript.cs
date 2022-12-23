using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.RuntimeScripts;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Damage;

public class DamageScript : BasicSpellScriptBase
{
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    protected Element? OffensiveElement { get; init; }
    protected int? ManaSpent { get; init; }
    protected bool Missed { get; set; }
    protected Animation MissedAnimation = new Animation { AnimationSpeed = 100, TargetAnimation = 115 };


    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject) { }

    protected virtual void ApplyDamage(SpellContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            if (!Randomizer.RollChance(100 - target.StatSheet.EffectiveMagicResistance / 2))
            {
                target.Animate(MissedAnimation);
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Your {Subject.Template.Name} has missed!");
                Missed = true;
                return;
            }
            ApplyDamageScripts.Default.ApplyDamage(
            context.Source,
            context.Target,
            this,
            CalculateDamage(context, target));
        }
    }

    protected virtual int CalculateDamage(SpellContext context, Creature target)
    {
        var damage = BaseDamage ?? 0;

        if (DamageStat.HasValue)
        {
            var multiplier = DamageStatMultiplier ?? 1;

            damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
        }
        if (OffensiveElement.HasValue)
        {
            return DamageFormulae.Default.CalculateElemental(context.Source, target, damage, OffensiveElement.Value, target.StatSheet.DefenseElement);
        }
        else
            return DamageFormulae.Default.Calculate(context.Source, target, damage);
    }

    /// <inheritdoc />
    protected override IEnumerable<T> GetAffectedEntities<T>(SpellContext context, IEnumerable<IPoint> affectedPoints)
    {
        var entities = base.GetAffectedEntities<T>(context, affectedPoints);

        return entities;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (ManaSpent.HasValue)
        {
            //Require mana
            if (context.Source.StatSheet.CurrentMp < ManaSpent.Value)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                return;
            }
            //Subtract mana and update user
            context.Source.StatSheet.SubtractMp(ManaSpent.Value);
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        }

        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        PlaySound(context, affectedPoints);
        ApplyDamage(context, affectedEntities);

        if (!Missed)
            ShowAnimation(context, affectedPoints);
        if (Missed)
            Missed = false;
    }
}