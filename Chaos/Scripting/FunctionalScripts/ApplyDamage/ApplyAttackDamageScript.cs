using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    protected readonly IEffectFactory EffectFactory;
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

        var relation = source.DirectionalRelationTo(target);

        if (relation == target.Direction.Reverse())
            damage = (int)(damage * 1.5);
        else if (relation != target.Direction)
            damage = (int)(damage * 1.25);

        switch (target)
        {
            case Aisling aisling:
                if (ReflectDamage(source, aisling, damage)) return;

                ApplyDamageAndTriggerEvents(aisling, damage, source);
                break;
            case Monster monster:
                if (ReflectDamage(source, monster, damage)) return;

                ApplyDamageAndTriggerEvents(monster, damage, source);
                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(source, damage);
                break;
        }
    }

    private void ApplyDamageAndTriggerEvents(Creature creature, int damage, Creature source)
    {
        creature.StatSheet.SubtractHp(damage);
        creature.ShowHealth();
        creature.Script.OnAttacked(source, damage);
        
        if (creature is Aisling aisling)
            aisling.Client.SendAttributes(StatUpdateType.Vitality);

        if (!creature.IsAlive)
        {
            switch (creature)
            {
                case Aisling mAisling:
                    mAisling.Script.OnDeath(source);

                    break;
                case Monster mCreature:
                    mCreature.Script.OnDeath();
                    break;
            }
        }
    }

    private bool ReflectDamage(Creature source, Creature target, int damage)
    {
        if ((target.Status.HasFlag(Status.AsgallFaileas) && IntegerRandomizer.RollChance(70)) || (target.Status.HasFlag(Status.EarthenStance) && IntegerRandomizer.RollChance(20)))
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

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}