using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;
using NLog;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairAllItemsScript : DialogScriptBase
{
    private double RepairCost;
    private readonly ILogger<RepairAllItemsScript> Logger;
    
    public RepairAllItemsScript(Dialog subject, ILogger<RepairAllItemsScript> logger)
        : base(subject) =>
        Logger = logger;

    private double CalculateRepairCostForItem(Item item)
    {
        // Skip if item is not damaged
        if ((item.Template.MaxDurability == null)
            || (item.CurrentDurability == null)
            || (item.CurrentDurability.Value == item.Template.MaxDurability.Value))
            return 0;

        // Calculate damage percentage
        var damage = (float)item.CurrentDurability.Value / item.Template.MaxDurability.Value;

        // Calculate formula
        return item.Template.SellValue / (2.0 * (.8 * damage));
    }

    private void CalculateRepairs(Aisling source)
    {
        RepairCost = 0;

        foreach (var item in source.Equipment)
            RepairCost += CalculateRepairCostForItem(item);

        foreach (var item in source.Inventory)
            RepairCost += CalculateRepairCostForItem(item);
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_repairalliteminitial":
                OnDisplayingInitial(source);

                break;
            case "generic_repairallitemaccepted":
                OnDisplayingAccepted(source);

                break;
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        CalculateRepairs(source);

        if (!source.TryTakeGold((int)RepairCost))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {(int)RepairCost} gold.");

            return;
        }

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Entities.Gold)
              .WithProperty(source).WithProperty(Subject)
              .LogInformation("{@AislingName} has repaired all items for {@AmountGold}", source.Name, RepairCost);
        
        foreach (var repair in source.Equipment)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
                repair.LastWarningLevel = 100;
            }

        foreach (var repair in source.Inventory)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
                source.Inventory.Update(
                    repair.Slot,
                    _ =>
                    {
                        repair.CurrentDurability = repair.Template.MaxDurability;
                        repair.LastWarningLevel = 100;
                    });
        
        
        source.SendOrangeBarMessage("Your items have been repaired.");
        Subject.InjectTextParameters((int)RepairCost);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        CalculateRepairs(source);

        if (RepairCost == 0)
        {
            Subject.Reply(source, "All of your items are already fully repaired!");

            return;
        }

        Subject.InjectTextParameters((int)RepairCost);
    }
}