using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairSingleItemScript(Dialog subject, ILogger<RepairSingleItemScript> logger) : DialogScriptBase(subject)
{
    private double RepairCost { get; set; }
    
    private int CalculateNewRepairCostForItem(Aisling aisling, Item item)
    {
        // Skip if item is not damaged
        if ((item.Template.MaxDurability == null) || (item.CurrentDurability == null) ||
            (item.CurrentDurability.Value == item.Template.MaxDurability.Value))
            return 0;

        const double REPAIR_FACTOR = 0.8;
        const double GUILD_HALL_DISCOUNT = 0.9;

        double sellValue = item.Template.SellValue;
        var damageProportion = 1 - (double)item.CurrentDurability.Value / item.Template.MaxDurability.Value;

        var repairCost = sellValue * damageProportion * REPAIR_FACTOR;

        // Apply multiplier for mythic items
        if (item.Template.TemplateKey.StartsWith("mythic", StringComparison.OrdinalIgnoreCase))
            repairCost *= 10;

        if (item.Template.TemplateKey.EndsWith("glove", StringComparison.OrdinalIgnoreCase))
            repairCost *= 126;
        
        // Apply guild hall discount
        if (aisling.MapInstance.BaseInstanceId == "guildhallmain")
            repairCost *= GUILD_HALL_DISCOUNT;

        return Convert.ToInt32(repairCost);
    }
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_repairsingleiteminitial":
                OnDisplayingInitial(source);

                break;
            case "generic_repairsingleitemconfirmation":
                OnDisplayingConfirmation(source);

                break;
            case "generic_repairsingleitemaccepted":
                OnDisplayingAccepted(source);

                break;
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (item is { CurrentDurability: not null, Template.MaxDurability: not null })
        {
            RepairCost = CalculateNewRepairCostForItem(source, item);

            if (!source.TryTakeGold((int)RepairCost))
            {
                Subject.Close(source);
                source.SendOrangeBarMessage($"You do not have enough gold. You need {RepairCost} gold.");

                return;
            }

            logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Entities.Gold
                      ])
                  .WithProperty(source)
                  .WithProperty(Subject)
                  .LogInformation(
                      "{@AislingName} has repaired {@ItemName} for {@AmountGold}",
                      source.Name,
                      item.DisplayName,
                      RepairCost);

            source.Inventory.Update(
                slot,
                item1 =>
                {
                    item1.CurrentDurability = item1.Template.MaxDurability;
                    item1.LastWarningLevel = 100;
                });

            source.SendOrangeBarMessage($"Your {item.DisplayName} has been repaired.");
            Subject.InjectTextParameters(item.DisplayName, RepairCost);
            source.Client.SendSound(172, false);
        }
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (item is { CurrentDurability: not null, Template.MaxDurability: not null })
        {
            RepairCost = CalculateNewRepairCostForItem(source, item);

            Subject.InjectTextParameters(item.DisplayName, RepairCost);
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        var itemsToRepair = source.Inventory
                                  .Where(
                                      x => (x.Template.MaxDurability != null)
                                           && (x.CurrentDurability != null)
                                           && (x.CurrentDurability.Value != x.Template.MaxDurability.Value))
                                  .ToList();

        if (itemsToRepair.Count == 0)
        {
            Subject.Reply(source, "All of your inventory items are already fully repaired!");

            return;
        }

        Subject.Slots = itemsToRepair.Select(x => x.Slot)
                                     .ToList();
    }
}