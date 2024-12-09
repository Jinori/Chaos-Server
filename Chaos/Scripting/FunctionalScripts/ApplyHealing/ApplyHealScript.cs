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

    /// <inheritdoc />
    public virtual void ApplyHeal(
        Creature source,
        Creature target,
        IScript script,
        int healing)
    {
        healing = HealFormula.Calculate(
            source,
            target,
            script,
            healing);

        if (healing <= 0)
            return;

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

                if (source.StatSheet.EffectiveHealBonus > 0)
                    healing += source.StatSheet.EffectiveHealBonus;

                if (source.StatSheet.EffectiveHealBonus > 0)
                {
                    var healBonusPct = healing * (source.StatSheet.EffectiveHealBonusPct / 100);
                    healing += healing * healBonusPct;
                }

                aisling.StatSheet.AddHp(healing);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                aisling.Script.OnHealed(source, healing);

                break;
            case Monster monster:

                if (source.StatSheet.EffectiveHealBonus > 0)
                    healing += source.StatSheet.EffectiveHealBonus;

                if (source.StatSheet.EffectiveHealBonus > 0)
                {
                    var healBonusPct = healing * (source.StatSheet.EffectiveHealBonusPct / 100);
                    healing += healing * healBonusPct;
                }

                monster.StatSheet.AddHp(healing);
                monster.ShowHealth();
                monster.Script.OnHealed(source, healing);

                break;
            case Merchant merchant:

                if (source.StatSheet.EffectiveHealBonus > 0)
                    healing += source.StatSheet.EffectiveHealBonus;

                if (source.StatSheet.EffectiveHealBonus > 0)
                {
                    var healBonusPct = healing * (source.StatSheet.EffectiveHealBonusPct / 100);
                    healing += healing * healBonusPct;
                }

                merchant.Script.OnHealed(source, healing);

                break;
        }
    }

    /// <inheritdoc />
    public static IApplyHealScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyHealScript>(Key);
}