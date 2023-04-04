using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairItemScript : DialogScriptBase
{
    private double _repairCost;

    public RepairItemScript(Dialog subject)
        : base(subject)
    {
        
    }

public override void OnDisplaying(Aisling source)
{
    // Calculate repair cost for equipment
    foreach (var item in source.Equipment)
    {
        // Skip if item is not damaged
        if (item.Template.MaxDurability == null || item.CurrentDurability == null || item.CurrentDurability.Value == item.Template.MaxDurability.Value)
            continue;
         // Calculate damage percentage
        var damage = ((float)item.CurrentDurability.Value / item.Template.MaxDurability.Value);
         // Calculate formula
        var formula = item.Template.SellValue / 2.0 * (.8 * damage);
         // Add to total repair cost
        _repairCost += formula;
    }
     // Calculate repair cost for inventory
    foreach (var item2 in source.Inventory)
    {
        // Skip if item is not damaged
        if (item2.Template.MaxDurability == null || item2.CurrentDurability == null || item2.CurrentDurability.Value == item2.Template.MaxDurability.Value)
            continue;
         // Calculate damage percentage
        var damage = ((float)item2.CurrentDurability.Value / item2.Template.MaxDurability.Value);
         // Calculate formula
        var formula = item2.Template.SellValue / 2.0 * (.8 * damage);
         // Add to total repair cost
        _repairCost += formula;
    }
     // Set subject text based on total repair cost
    if (_repairCost != 0)
        Subject.Reply(source, $"I can repair all of your items for {(int)_repairCost}. Would you like to continue?");
    else
    {
        Subject.Reply(source, "I cannot repair your items any further.");
    }
}

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!source.TryTakeGold((int)_repairCost))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {(int)_repairCost} gold.");
            return;
        }

        foreach (var repair in source.Equipment)
        {
            if (repair.Template.MaxDurability > 0 && repair.CurrentDurability != repair.Template.MaxDurability)
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
            }
        }

        foreach (var repair in source.Inventory)
        {
            if (repair.Template.MaxDurability > 0 && repair.CurrentDurability != repair.Template.MaxDurability)
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
            }
        }

        source.SendOrangeBarMessage($"Your items have been repaired.");
    }
}