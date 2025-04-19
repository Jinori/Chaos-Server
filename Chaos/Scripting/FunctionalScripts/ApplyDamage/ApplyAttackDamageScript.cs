#region
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
using Chaos.Scripting.EffectScripts.Monk;
using Chaos.Scripting.EffectScripts.Warrior;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.ApplyDamage;

public class ApplyAttackDamageScript(IEffectFactory effectFactory, ILogger<ApplyAttackDamageScript> logger) : ScriptBase, IApplyDamageScript
{
    protected readonly IEffectFactory EffectFactory = effectFactory;
    public IDamageFormula DamageFormula { get; set; } = DamageFormulae.Default;
    public static string Key { get; } = GetScriptKey(typeof(ApplyAttackDamageScript));

    public virtual int ApplyDamage(
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
            return 0;

        if (!source.OnSameMapAs(target))
            return 0;

        target.Trackers.LastDamagedBy = source;

        if (ReflectDamage(source, target, script, damage))
            return damage;

        if (target is Aisling aislingTarget)
            ApplyDurabilityDamage(aislingTarget, source, script);

        ApplyDamageAndTriggerEvents(source, target, script, damage);

        return damage;
    }

    public static IApplyDamageScript Create() => FunctionalScriptRegistry.Instance.Get<IApplyDamageScript>(Key);

    private void ApplyDamageAndTriggerEvents(
        Creature source,
        Creature target,
        IScript script,
        int damage)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (target is Merchant)
        {
            target.Script.OnAttacked(source, damage);

            return;
        }

        //Pets cannot be damaged by Aislings
        if (target is Monster { PetOwner: not null } && source is Aisling)
            return;

        if (!target.IsAlive || target.IsDead)
            return;

        if ((damage > target.StatSheet.CurrentHp) && target.IsInLastStand())
            damage = target.StatSheet.CurrentHp - 1;

        target.StatSheet.SubtractHp(damage);
        target.ShowHealth();
        target.Script.OnAttacked(source, damage);

        if (target is Aisling aisling)
            aisling.Client.SendAttributes(StatUpdateType.Vitality);

        if (!target.IsAlive)
            switch (target)
            {
                case Aisling mAisling:
                    mAisling.Script.OnDeath();

                    break;
                case Monster mCreature:
                    mCreature.Script.OnDeath();

                    break;
            }
        else if (source.Effects.TryGetEffect("Thunder Stance", out var effect)
                 && effect is ThunderStanceEffect thunderStanceEffect
                 && script is not ThunderStanceEffect
                 && !IsAssail(script))
            thunderStanceEffect.ApplyDamage(target, damage);
        else if (source.Effects.TryGetEffect("Lightning Stance", out var effect2)
                 && effect2 is LightningStanceEffect lightningStanceEffect
                 && script is not LightningStanceEffect
                 && !IsAssail(script))
            lightningStanceEffect.ApplyDamage(target, damage);
    }

    protected virtual bool IsAssail(IScript source)
    {
        if (source is SubjectiveScriptBase<Skill> { Subject.Template.IsAssail: true } or WrathEffect or WhirlwindEffect or InfernoEffect)
            return true;

        return false;
    }
    
    private void ApplyDurabilityDamage(Aisling aisling, Creature source, IScript skillSource)
    {
        if (skillSource is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (aisling.IsOnArenaMap() || aisling.IsGodModeEnabled())
            return;

        if (source.MapInstance is { IsShard: true, LoadedFromInstanceId: "guildhallmain" })
            return;

        if (!skillScript.Subject.Template.IsAssail)
            return;

        foreach (var item in aisling.Equipment)
        {
            if (item.Slot is <= 0 or >= 14)
                continue;

            if (item.CurrentDurability is >= 1)
                item.CurrentDurability--;

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

    private bool ReflectDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage)
    {
        if ((source.IsConfused() && IntegerRandomizer.RollChance(25) && !source.IsGodModeEnabled())
            || (target.IsAsgalled() && IntegerRandomizer.RollChance(50))
            || (target.IsEarthenStanced() && IntegerRandomizer.RollChance(30))
            || (target.IsRockStanced() && IntegerRandomizer.RollChance(70)))
        {
            ApplyDamageAndTriggerEvents(
                source,
                source,
                script,
                damage);

            return true;
        }

        return false;
    }
}