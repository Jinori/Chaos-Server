using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class VerbalRepairAllScript : MerchantScriptBase
{

    private readonly ILogger<VerbalRepairAllScript> Logger;

    /// <inheritdoc />
    public VerbalRepairAllScript(Merchant subject, ILogger<VerbalRepairAllScript> logger)
        : base(subject)
        => Logger = logger;

    private double RepairCost;

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        if (source is not Aisling aisling)
            return;

        if (!message.EqualsI("Repair All"))
            return;

        CalculateRepairs(aisling);
        
        if (RepairCost == 0)
        {
            aisling.SendOrangeBarMessage("Your items are already repaired.");
            Subject.Say($"Your items are already repaired {aisling.Name}.");

            return;
        }

        if (!aisling.TryTakeGold((int)RepairCost))
        {
            aisling.SendOrangeBarMessage($"You do not have enough. You need {(int)RepairCost} gold.");
            Subject.Say($"You don't have enough gold {aisling.Name} to repair!");

            return;
        }

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Entities.Gold
                  ])
              .WithProperty(aisling)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} has repaired all items for {@AmountGold}", aisling.Name, RepairCost);

        foreach (var repair in aisling.Equipment)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
                repair.LastWarningLevel = 100;
            }

        foreach (var repair in aisling.Inventory)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
                aisling.Inventory.Update(
                    repair.Slot,
                    _ =>
                    {
                        repair.CurrentDurability = repair.Template.MaxDurability;
                        repair.LastWarningLevel = 100;
                    });

        aisling.SendOrangeBarMessage($"Your items have been repaired for {RepairCost} gold.");
        Subject.Say($"There you go {aisling.Name}, your items are as good as new.");
        aisling.Client.SendSound(172, false);


    }

    private void CalculateRepairs(Aisling source)
    {
        RepairCost = 0;

        foreach (var item in source.Equipment)
            RepairCost += CalculateNewRepairCostForItem(source, item);

        foreach (var item in source.Inventory)
            RepairCost += CalculateNewRepairCostForItem(source, item);
    }

    private int CalculateNewRepairCostForItem(Aisling aisling, Item item)
    {
        // Skip if item is not damaged
        if ((item.Template.MaxDurability == null)
            || (item.CurrentDurability == null)
            || (item.CurrentDurability.Value == item.Template.MaxDurability.Value))
            return 0;

        const double REPAIR_FACTOR = 0.8;
        const double GUILD_HALL_DISCOUNT = 0.9;

        double sellValue = item.Template.SellValue;
        var damageProportion = 1 - (double)item.CurrentDurability.Value / item.Template.MaxDurability.Value;

        var repairCost = sellValue * damageProportion * REPAIR_FACTOR;

        // Apply multiplier for mythic items
        if (item.Template.TemplateKey.StartsWith("mythic", StringComparison.OrdinalIgnoreCase))
            repairCost *= 10;

        // Apply guild hall discount
        if (aisling.MapInstance.LoadedFromInstanceId == "guildhallmain")
            repairCost *= GUILD_HALL_DISCOUNT;

        return Convert.ToInt32(repairCost);
    }
}