using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyHealing;

public class ApplyHealScript : ScriptBase, IApplyHealScript
{
    /// <inheritdoc />
    public IHealFormula HealFormula { get; set; } = HealFormulae.Default;

    public static string Key { get; } = GetScriptKey(typeof(ApplyHealScript));

    private int ApplyHealingBonuses(Creature source, int baseHealing)
    {
        if (baseHealing <= 0)
            return 0;

        var healing = baseHealing;

        // Flat heal bonus
        if (source.StatSheet.EffectiveHealBonus > 0)
            healing += source.StatSheet.EffectiveHealBonus;

        // Percent heal bonus
        if (source.StatSheet.EffectiveHealBonusPct > 0)
        {
            var bonusPct = source.StatSheet.EffectiveHealBonusPct / 100m;
            healing += (int)(healing * bonusPct);
        }

        return healing;
    }
    
    public virtual void ApplyHeal(
        Creature source,
        Creature target,
        IScript script,
        int healing)
    {
        healing = HealFormula.Calculate(source, target, script, healing);

        if (healing <= 0)
            return;

        healing = ApplyHealingBonuses(source, healing);

        switch (target)
        {
            case Aisling aisling:
                if (aisling.IsDead)
                    return;

                if (aisling.Effects.Contains("Prevent Heal"))
                {
                    aisling.SendOrangeBarMessage("You're currently preventing heals.");
                    return;
                }

                aisling.StatSheet.AddHp(healing);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                aisling.Script.OnHealed(source, healing);
                break;

            case Monster monster:
                monster.StatSheet.AddHp(healing);
                monster.ShowHealth();
                monster.Script.OnHealed(source, healing);
                break;

            case Merchant merchant:
                merchant.Script.OnHealed(source, healing);
                break;
        }
    }

    /// <inheritdoc />
    public static IApplyHealScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyHealScript>(Key);
}