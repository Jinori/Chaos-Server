using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.PlayerDeath;

namespace Chaos.Scripts.FunctionalScripts.ApplyDamage;

public class ExecuteApplyDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public IPlayerDeathScript PlayerDeathScript { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(DefaultApplyDamageScript));

    public ExecuteApplyDamageScript()
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
        
        var tenPercent = Convert.ToInt32(.1 * defender.StatSheet.EffectiveMaximumHp);
        damage = DamageFormula.Calculate(attacker, defender, tenPercent);

        switch (defender)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                
                if (!aisling.IsAlive)
                    PlayerDeathScript.OnDeath(aisling, attacker);
                break;
            
            case Monster monster:
                monster.StatSheet.SubtractHp(damage);
                monster.ShowHealth();
                monster.Script.OnAttacked(attacker, damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();
                break;
            
            case Merchant merchant:
                merchant.Script.OnAttacked(attacker, damage);
                break;
        }
        
        var tenPercentAfterDamage = Convert.ToInt32(.1 * defender.StatSheet.EffectiveMaximumHp);
        
        switch (attacker)
        {
            
            case Aisling aisling:
                if (tenPercentAfterDamage >= defender.StatSheet.CurrentHp)
                {
                    defender.StatSheet.SubtractHp(defender.StatSheet.CurrentHp);
                    var fivePercent = Convert.ToInt32(.05 * aisling.StatSheet.EffectiveMaximumHp);
                    aisling.ApplyHealing(attacker, fivePercent);
                    aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    aisling.SkillBook["Execute"]!.Cooldown = new TimeSpan(0, 0, 15);
                }
                break;
            
            case Monster monster:
                if (tenPercentAfterDamage >= defender.StatSheet.CurrentHp)
                {
                    defender.StatSheet.SubtractHp(defender.StatSheet.CurrentHp);
                    var fivePercent = Convert.ToInt32(.05 * monster.StatSheet.EffectiveMaximumHp);
                    monster.ApplyHealing(attacker, fivePercent);
                    monster.ShowHealth();
                }
                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}