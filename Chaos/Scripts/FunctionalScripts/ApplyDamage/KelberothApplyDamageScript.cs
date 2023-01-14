using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.PlayerDeath;

namespace Chaos.Scripts.FunctionalScripts.ApplyDamage;

public class KelberothApplyDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public IPlayerDeathScript PlayerDeathScript { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(DefaultApplyDamageScript));

    public KelberothApplyDamageScript()
    {
        DamageFormula = DamageFormulae.Default;
        PlayerDeathScript = DefaultPlayerDeathScript.Create();
    }

    public virtual void ApplyDamage(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    )
    {
        damage = DamageFormula.Calculate(attacker, defender, damage);
        var kelbAmount = Convert.ToInt32(.3 * attacker.StatSheet.CurrentHp) + damage;

        switch (attacker)
        {
            case Aisling aisling:
                var sacAislingHealth = Convert.ToInt32(.6 * aisling.StatSheet.CurrentHp);
                aisling.StatSheet.SubtractHp(sacAislingHealth);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                break;
            
            case Monster monster:
                var sacMonsterHealth = Convert.ToInt32(.6 * monster.StatSheet.CurrentHp);
                monster.StatSheet.SubtractHp(sacMonsterHealth);
                monster.ShowHealth();
                break;
        }

        switch (defender)
        {
            case Aisling aisling:
                
                aisling.StatSheet.SubtractHp(kelbAmount);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                
                if (!aisling.IsAlive)
                    PlayerDeathScript.OnDeath(aisling, attacker);
                break;
            
            case Monster monster:
                monster.StatSheet.SubtractHp(kelbAmount);
                monster.ShowHealth();
                monster.Script.OnAttacked(attacker, damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();
                break;
            
            case Merchant merchant:
                merchant.Script.OnAttacked(attacker, kelbAmount);
                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}