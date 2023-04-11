using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairAllItemsScript : DialogScriptBase
{
    public RepairAllItemsScript(Dialog subject) : base(subject)
    {
        
    }
    
    private double RepairCost;
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_repairalliteminitial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_repairallitemaccepted":
            {
                OnDisplayingAccepted(source);
                break;
            }   
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
                source.Inventory.Update(repair.Slot, item1 =>
                {
                    repair.CurrentDurability = repair.Template.MaxDurability;
                });
            }
        }

        source.SendOrangeBarMessage($"Your items have been repaired.");
        Subject.InjectTextParameters((int)RepairCost);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        CalculateRepairs(source);
        Subject.InjectTextParameters((int)RepairCost);
    }
    

    private void CalculateRepairs(Aisling source)
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
            var formula = item.Template.SellValue / (2.0 * (.8 * damage));
            // Add to total repair cost
            RepairCost += formula;
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
            var formula = item2.Template.SellValue / (2.0 * (.8 * damage));
            // Add to total repair cost
            RepairCost += formula;
        }
    }
}