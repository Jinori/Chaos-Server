using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript(IEffectFactory effectFactory, ILogger<ApplyAttackDamageScript> logger) : ScriptBase, IApplyDamageScript
{
    protected readonly IEffectFactory EffectFactory = effectFactory;
    public IDamageFormula DamageFormula { get; set; } = DamageFormulae.Default;
    public static string Key { get; } = GetScriptKey(typeof(ApplyAttackDamageScript));

    public virtual void ApplyDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null)
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

        switch (target)
        {
            case Aisling aisling:
                
                var relation = target.DirectionalRelationTo(source);
                
                if (aisling.UserStatSheet.BaseClass is BaseClass.Monk)
                {
                    if (relation == target.Direction.Reverse())
                        damage = (int)(damage * 1.25);
                }
                else
                {
                    if (relation == target.Direction.Reverse())
                        damage = (int)(damage * 1.5);
                    else if (relation != target.Direction)
                        damage = (int)(damage * 1.25);   
                }
                
                if (ReflectDamage(source, aisling, damage))
                    return;
                
                
                ApplyDamageAndTriggerEvents(aisling, damage, source);
                ApplyDurabilityDamage(aisling, source, script);
                
                break;
            case Monster monster:
                var relation1 = target.DirectionalRelationTo(source);
                                
                if (relation1 == target.Direction.Reverse())
                    damage = (int)(damage * 1.5);
                else if (relation1 != target.Direction)
                    damage = (int)(damage * 1.25);  
                
                
                if (ReflectDamage(source, monster, damage))
                    return;

                ApplyDamageAndTriggerEvents(monster, damage, source);

                break;
            case Merchant merchant:
                merchant.Script.OnAttacked(source, damage);

                break;
        }
    }

    private void ApplyDurabilityDamage(Aisling aisling, Creature source, IScript skillSource)
    {
        if (skillSource is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (skillScript.Subject.Template.IsAssail)
        {
            foreach (var item in aisling.Equipment)
            {
                
                if (aisling.IsGodModeEnabled())
                    continue;

                if (item.Slot is > 0 and < 14)
                {
                    if (item.CurrentDurability >= 1)
                        item.CurrentDurability--;

                    var dura = GetCurrentDurabilityPercentage(item);
                    HandleDurabilityWarning(aisling, item, dura);
                    HandleBreakingItem(aisling, source, item);
                }
            }
        }
    }

    private int? GetCurrentDurabilityPercentage(Item item) =>
        MathEx.CalculatePercent<int>(item.CurrentDurability!.Value, item.Template.MaxDurability!.Value);

    private int GetWarningLevel(int? percent) =>
        percent switch
        {
            <= 5  => 5,
            <= 10 => 10,
            <= 30 => 30,
            <= 50 => 50,
            _     => 0
        };

    private void HandleDurabilityWarning(Aisling target, Item item, int? percent)
    {
        var warningLevel = GetWarningLevel(percent);
        
        if ((warningLevel > 0) && (warningLevel != item.LastWarningLevel))
        {
            target.SendActiveMessage($"{percent}% durability reached on {item.DisplayName}.");
            item.LastWarningLevel = warningLevel;
        }
    }

    private void HandleBreakingItem(Aisling target, Creature source, Item item)
    {
        if (item is { CurrentDurability: <= 0, Template.AccountBound: false })
        {
            target.SendActiveMessage($"Your {item.DisplayName} broke.");
            target.Equipment.TryGetRemove(item.Slot, out _);
            HandleLoggingItem(target, source, item, false);
        }

        if (item is { CurrentDurability: <= 0, Template.AccountBound: true })
            if (target.Equipment.TryGetRemove(item.Slot, out var removedItem))
            {
                if (target.CanCarry(removedItem))
                    target.Inventory.TryAddToNextSlot(removedItem);
                else
                {
                    target.Bank.Deposit(removedItem);
                    target.SendActiveMessage($"{item.DisplayName} was nearly broken and sent to your bank.");
                    HandleLoggingItem(target, source, item, true);
                }
            }
    }
    
    private void HandleLoggingItem(Aisling aisling, Creature source, Item item, bool banked)
    {
        if (banked)
        {
            logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Actions.Deposit,
                      Topics.Actions.Penalty)
                  .WithProperty(aisling)
                  .WithProperty(item)
                  .LogInformation("{@AislingName} almost broke {@ItemName} through durability {@SourceName} but it was sent to the bank", aisling.Name, item.DisplayName, source.Name);
        }
        else
        {
            logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Actions.Remove,
                      Topics.Actions.Penalty)
                  .WithProperty(aisling)
                  .WithProperty(item)
                  .LogInformation("{@AislingName} broke {@ItemName} through durability from {@SourceName}", aisling.Name, item.DisplayName, source.Name);
        }
    }
    
    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);

    private void ApplyDamageAndTriggerEvents(Creature creature, int damage, Creature source)
    {
        //Pets cannot be damaged by Aislings
        if (creature is Monster { PetOwner: not null } && source is Aisling)
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
        if ((target.IsAsgalled() && IntegerRandomizer.RollChance(70))
            || (target.IsEarthenStanced() && IntegerRandomizer.RollChance(20)))
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

        if (target.IsSmokeStanced() && IntegerRandomizer.RollChance(15) && source is Monster monsterSource)
        {
            var effect = EffectFactory.Create("Blind");
            monsterSource.Effects.Apply(target, effect);
        }

        return false;
    }
}