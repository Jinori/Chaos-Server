using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript : ScriptBase, IApplyDamageScript
{
    protected readonly IEffectFactory EffectFactory;
    public IDamageFormula DamageFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(ApplyAttackDamageScript));

    public ApplyAttackDamageScript(IEffectFactory effectFactory)
    {
        DamageFormula = DamageFormulae.Default;
        EffectFactory = effectFactory;
    }

    public virtual void ApplyDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null
    )
    {
        damage = DamageFormula.Calculate(
            source,
            target,
            script,
            damage,
            elementOverride);

        if (damage <= 0)
            return;

        if (!source.OnSameMapAs(target))
            return;

        target.Trackers.LastDamagedBy = source;

        var relation = source.DirectionalRelationTo(target);

        if (relation == target.Direction.Reverse())
            damage = (int)(damage * 1.5);
        else if (relation != target.Direction)
            damage = (int)(damage * 1.25);

        switch (target)
        {
            case Aisling aisling:
                if (ReflectDamage(source, aisling, damage))
                    return;

                ApplyDamageAndTriggerEvents(aisling, damage, source);

                break;
            case Monster monster:
                if (ReflectDamage(source, monster, damage))
                    return;

                ApplyDamageAndTriggerEvents(monster, damage, source);

                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(source, damage);

                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);

    private void ApplyDamageAndTriggerEvents(Creature creature, int damage, Creature source)
    {
        //Pet owners cannot damage their pet
        if (creature is Monster monster && source is Aisling owner && (monster.PetOwner != null) && monster.PetOwner.Equals(owner))
            return;

        if (!creature.IsAlive || creature.IsDead)
            return;

        creature.StatSheet.SubtractHp(damage);
        creature.ShowHealth();
        creature.Script.OnAttacked(source, damage);

        if (creature is Aisling aisling)
            aisling.Client.SendAttributes(StatUpdateType.Vitality);

        if (!creature.IsAlive)
            switch (creature)
            {
                case Aisling mAisling:
                    mAisling.Script.OnDeath();

                    break;
                case Monster mCreature:
                    mCreature.Script.OnDeath();

                    break;
            }
    }

    private bool ReflectDamage(Creature source, Creature target, int damage)
    {
        if ((target.Status.HasFlag(Status.AsgallFaileas) && IntegerRandomizer.RollChance(70))
            || (target.Status.HasFlag(Status.EarthenStance) && IntegerRandomizer.RollChance(20)))
        {
            switch (source)
            {
                case Aisling sourceAisling:
                    ApplyDamageAndTriggerEvents(sourceAisling, damage, target);

                    break;
                case Monster monster:
                    ApplyDamageAndTriggerEvents(monster, damage, target);

                    break;
            }

            return true;
        }

        if (target.Status.HasFlag(Status.SmokeStance) && IntegerRandomizer.RollChance(15) && source is Monster monsterSource)
        {
            var effect = EffectFactory.Create("Blind");
            monsterSource.Effects.Apply(target, effect);
        }

        return false;
    }
}