using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(ApplyAttackDamageScript));

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 9
    };
    

    public ApplyAttackDamageScript()
    {
        DamageFormula = DamageFormulae.Default;
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

        switch (target)
        {
            case Aisling aisling:
                if (aisling.Status.HasFlag(Status.AsgallFaileas) && Randomizer.RollChance(70) || aisling.Status.HasFlag(Status.EarthenStance) && Randomizer.RollChance(20))
                {
                    switch (source)
                    {
                        case Aisling sourceAisling:
                        {
                            sourceAisling.StatSheet.SubtractHp(damage);
                            sourceAisling.Client.SendAttributes(StatUpdateType.Vitality);   
                            sourceAisling.ShowHealth();
                            sourceAisling.Script.OnAttacked(target, damage);
                            if (!sourceAisling.IsAlive)
                                sourceAisling.Script.OnDeath(target);
                            break;
                        }
                        case Monster monster:
                        {
                            monster.StatSheet.SubtractHp(damage);
                            monster.ShowHealth();
                            monster.Script.OnAttacked(target, damage);
                            if (!monster.IsAlive)
                                monster.Script.OnDeath();
                            break;
                        }
                    }

                    return;
                }
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                aisling.Script.OnAttacked(source, damage);

                if (aisling.Status.HasFlag(Status.MistStance))
                {
                    var result = damage * .15m;
                    if (aisling.Group is not null)
                    {
                        foreach (var person in aisling.Group)
                        {
                            person.Animate(MistHeal, person.Id);
                            person.ApplyHealing(aisling, (int)result);
                        }
                    }
                    else
                    {
                        aisling.Animate(MistHeal, aisling.Id);
                        aisling.ApplyHealing(aisling, (int)result);
                    }
                }
                if (!aisling.IsAlive)
                    aisling.Script.OnDeath(source);

                break;
            case Monster monster:
                if (monster.Status.HasFlag(Status.AsgallFaileas) && Randomizer.RollChance(70) || monster.Status.HasFlag(Status.EarthenStance) && Randomizer.RollChance(20))
                {
                    switch (source)
                    {
                        case Aisling sourceAisling:
                        {
                            sourceAisling.StatSheet.SubtractHp(damage);
                            sourceAisling.Client.SendAttributes(StatUpdateType.Vitality);   
                            sourceAisling.ShowHealth();
                            sourceAisling.Script.OnAttacked(target, damage);
                            if (!sourceAisling.IsAlive)
                                sourceAisling.Script.OnDeath(target);
                            break;
                        }
                        case Monster mob:
                        {
                            mob.StatSheet.SubtractHp(damage);
                            mob.ShowHealth();
                            mob.Script.OnAttacked(target, damage);
                            if (!mob.IsAlive)
                                mob.Script.OnDeath();
                            break;
                        }
                    }

                    return;
                }
                monster.StatSheet.SubtractHp(damage);
                monster.ShowHealth();
                monster.Script.OnAttacked(source, damage);

                if (monster.Status.HasFlag(Status.MistStance))
                {
                    var result = damage * .15m;
                    monster.Animate(MistHeal, monster.Id);
                    monster.ApplyHealing(monster, (int)result);
                }
                if (!monster.IsAlive)
                    monster.Script.OnDeath();

                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(source, damage);

                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}