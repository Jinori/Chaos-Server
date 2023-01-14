using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.PlayerDeath;

namespace Chaos.Scripts.FunctionalScripts.ApplyDamage;

public class SapNeedleApplyDamageScript : ScriptBase, IApplyDamageScript
{
    public IDamageFormula DamageFormula { get; set; }
    public IPlayerDeathScript PlayerDeathScript { get; set; }
    public static string Key { get; } = GetScriptKey(typeof(DefaultApplyDamageScript));
    
    protected readonly Animation SuccessfulSap = new Animation { AnimationSpeed = 100, TargetAnimation = 127 };

    public SapNeedleApplyDamageScript()
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
        var defenderMana = Convert.ToInt32(defender.StatSheet.CurrentHp);
        int twentyPercent = Convert.ToInt32(0.20 * defenderMana);
        
        switch (attacker)
        {
            case Aisling aisling:
                var group = aisling.Group?.Where(x => x.WithinRange(aisling));
                if (group is not null)
                {
                    defender.StatSheet.SubtractMp(defenderMana);
                    foreach (var member in group)
                    {
                        member.ApplyMana(member,defenderMana / group.Count());
                        member.Client.SendAttributes(StatUpdateType.Vitality);
                        member.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(aisling.Id));
                    }
                }
                else
                {
                    defender.StatSheet.SubtractMp(defenderMana);
                    aisling.ApplyMana(aisling, defenderMana / 2);
                    aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    aisling.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(aisling.Id));
                }
                break;
            
            case Monster monster:
                monster.ApplyMana(monster, twentyPercent);
                monster.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(monster.Id));
                break;
        }

        switch (defender)
        {
            case Aisling aisling:
                aisling.StatSheet.SubtractHp(damage);
                aisling.StatSheet.SubtractMp(twentyPercent);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.ShowHealth();
                
                if (!aisling.IsAlive)
                    PlayerDeathScript.OnDeath(aisling, attacker);
                break;
            
            case Monster monster:
                monster.StatSheet.SubtractHp(damage);
                monster.StatSheet.SubtractMp(defenderMana);
                monster.ShowHealth();
                monster.Script.OnAttacked(attacker, damage);

                if (!monster.IsAlive)
                    monster.Script.OnDeath();
                break;
            
            case Merchant merchant:
                merchant.Script.OnAttacked(attacker, damage);

                break;
        }
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);
}