using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
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

    private readonly List<string> MapsToNotPunishDurability =
    [
        "Labyrinth Battle Ring",
        "Drowned Labyrinth - Pit",
        "Lava Arena",
        "Lava Arena - Teams",
        "Color Clash - Teams",
        "Hidden Havoc",
        "Escort - Teams"
    ];
    
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

                var relation = source.DirectionalRelationTo(target);

                if (aisling.Effects.Contains("Dodge") || aisling.Effects.Contains("Evasion"))
                {
                    if (aisling.Effects.Contains("Dodge"))
                    {
                        if (relation == target.Direction.Reverse())
                            damage = (int)(damage * 1.4);
                        else if (relation != target.Direction)
                            damage = (int)(damage * 1.2);
                    }

                    if (aisling.Effects.Contains("Evasion"))
                    {
                        if (relation == target.Direction.Reverse())
                            damage = (int)(damage * 1.3);
                        else if (relation != target.Direction)
                            damage = (int)(damage * 1.1);
                    }
                } else
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
                var relation1 = source.DirectionalRelationTo(target);

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

    private void ApplyDurabilityDamage(Aisling aisling, Creature source, IScript skillSource)
    {
        if (skillSource is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (MapsToNotPunishDurability.Contains(aisling.MapInstance.Name) || aisling.IsGodModeEnabled())
            return;

        if (source.MapInstance is { IsShard: true, BaseInstanceId: "guildhallmain" })
            return;
        

        if (!skillScript.Subject.Template.IsAssail) 
            return;
        
        foreach (var item in aisling.Equipment)
        {
            if (item.Slot is <= 0 or >= 14) 
                continue;
            
            if (item.CurrentDurability is >= 1)
            {
                item.CurrentDurability--;
            }

            if (!item.CurrentDurability.HasValue) 
                continue;
                
            var dura = GetCurrentDurabilityPercentage(item);
            HandleDurabilityWarning(aisling, item, dura);
            HandleBreakingItem(aisling, source, item);
        }
    }

    private int? GetCurrentDurabilityPercentage(Item item)
        => MathEx.CalculatePercent<int>(item.CurrentDurability!.Value, item.Template.MaxDurability!.Value);

    private int GetWarningLevel(int? percent)
        => percent switch
        {
            <= 5  => 5,
            <= 10 => 10,
            <= 30 => 30,
            <= 50 => 50,
            _     => 0
        };

    private void HandleBreakingItem(Aisling target, Creature source, Item item)
    {
        if (item is { CurrentDurability: <= 0, Template.AccountBound: false })
        {
            target.SendActiveMessage($"Your {item.DisplayName} broke.");
            target.Equipment.TryGetRemove(item.Slot, out _);

            HandleLoggingItem(
                target,
                source,
                item,
                false);
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

                    HandleLoggingItem(
                        target,
                        source,
                        item,
                        true);
                }
            }
    }

    private void HandleDurabilityWarning(Aisling target, Item item, int? percent)
    {
        var warningLevel = GetWarningLevel(percent);

        if ((warningLevel > 0) && (warningLevel != item.LastWarningLevel))
        {
            target.SendActiveMessage($"{percent}% durability reached on {item.DisplayName}.");
            item.LastWarningLevel = warningLevel;
        }
    }

    private void HandleLoggingItem(
        Aisling aisling,
        Creature source,
        Item item,
        bool banked)
    {
        if (banked)
            logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Actions.Deposit,
                      Topics.Actions.Penalty)
                  .WithProperty(aisling)
                  .WithProperty(item)
                  .LogInformation(
                      "{@AislingName} almost broke {@ItemName} through durability {@SourceName} but it was sent to the bank",
                      aisling.Name,
                      item.DisplayName,
                      source.Name);
        else
            logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Actions.Remove,
                      Topics.Actions.Penalty)
                  .WithProperty(aisling)
                  .WithProperty(item)
                  .LogInformation(
                      "{@AislingName} broke {@ItemName} through durability from {@SourceName}",
                      aisling.Name,
                      item.DisplayName,
                      source.Name);
    }

    private bool ReflectDamage(Creature source, Creature target, int damage)
    {
        if (source.IsConfused() && IntegerRandomizer.RollChance(100) && !source.IsGodModeEnabled())
            switch (source)
            {
                case Aisling sourceAisling:
                    ApplyDamageAndTriggerEvents(sourceAisling, damage, target);

                    break;
                case Monster monster:
                    ApplyDamageAndTriggerEvents(monster, damage, monster);

                    break;
            }

        if ((target.IsAsgalled() && IntegerRandomizer.RollChance(50))
            || (target.IsEarthenStanced() && IntegerRandomizer.RollChance(30))
            || (target.IsRockStanced() && IntegerRandomizer.RollChance(70)))
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

        return false;
    }
}