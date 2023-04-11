using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairSingleItemScript : DialogScriptBase
{
    public RepairSingleItemScript(Dialog subject) : base(subject)
    {
        
    }
    
    private int RepairCost { get; set; }
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_repairsingleiteminitial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_repairsingleitemconfirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_repairsingleitemaccepted":
            {
                OnDisplayingAccepted(source);
                break;
            }   
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var damage = ((float)item.CurrentDurability.Value / item.Template.MaxDurability.Value);
        var formula = item.Template.SellValue / 2.0 * (.8 * damage);
        RepairCost = (int)(RepairCost + formula);
        
        if (!source.TryTakeGold((int)RepairCost))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {(int)RepairCost} gold.");
            return;
        }
        
        source.Inventory.Update(slot, item1 =>
        {
            item.CurrentDurability = item.Template.MaxDurability;
        });
        
        source.SendOrangeBarMessage($"Your {item.DisplayName} has been repaired.");
        Subject.InjectTextParameters(item.DisplayName, RepairCost);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        
        var damage = ((float)item.CurrentDurability.Value / item.Template.MaxDurability.Value);
        var formula = item.Template.SellValue / 2.0 * (.8 * damage);
        RepairCost = (((int)(RepairCost + formula)));
        
        Subject.InjectTextParameters(item.DisplayName, RepairCost);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        Subject.Slots = source.Inventory.Where(x =>
            x.Template.MaxDurability != null && x.CurrentDurability != null &&
            x.CurrentDurability.Value != x.Template.MaxDurability.Value).Select(x => x.Slot).ToList();
    }
}