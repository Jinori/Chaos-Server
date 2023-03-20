using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairSingleItemScript : DialogScriptBase
{
    public RepairSingleItemScript(Dialog subject) : base(subject)
    {
        var requestInputText = DialogString.From(() => $"Would you like to repair {Item!.DisplayName} for {RepairCost} gold?");
        InputCollector = new InputCollectorBuilder()
            .RequestOptionSelection(requestInputText, DialogString.Yes, DialogString.No)
            .HandleInput(HandleInput)
            .Build();
    }

    private bool HandleInput(Aisling source, Dialog dialog, int? option)
    {
        if (option is not 1)
            return false;

        if (!source.TryTakeGold(RepairCost))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {RepairCost} gold.");
            return false;
        }

        if (Item != null) Item.CurrentDurability = Item.Template.MaxDurability;
        source.SendOrangeBarMessage($"Your item has been repaired.");
        return true;
    }

    private Item? Item { get; set; }
    private int RepairCost { get; set; }
    private InputCollector InputCollector { get; }
    

    public override void OnDisplaying(Aisling source)
    {
        var inventory = source.Inventory.Where(x =>
                x.Template.MaxDurability != null && x.CurrentDurability != null &&
                x.CurrentDurability.Value != x.Template.MaxDurability.Value)
            .ToList();
        if (Subject.Slots.IsNullOrEmpty())
            Subject.Slots = inventory.Select(x => x.Slot).ToList();
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Item != null)
        {
            if (!source.TryTakeGold(RepairCost))
            {
                Subject.Close(source);
                source.SendOrangeBarMessage($"You do not have enough. You need {RepairCost} gold.");
            }
            else
            {
                Item.CurrentDurability = Item.Template.MaxDurability;
                source.SendOrangeBarMessage($"Your item has been repaired.");
                source.Inventory[Item.Slot]?.Update(TimeSpan.FromSeconds(1));
                Subject.Close(source);
            }
        }

        if (Item != null)
            return;
        
        if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        var item = source.Inventory[slot];
        if (item == null)
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        Item = item;
        if (item.CurrentDurability == null || item.Template.MaxDurability == null)
            return;
        var damage = ((float)item.CurrentDurability.Value / item.Template.MaxDurability.Value);
        var formula = item.Template.SellValue / 2.0 * (.8 * damage);
        RepairCost = (((int)(RepairCost + formula)));
            
        if (source.Gold >= RepairCost)
        {
            InputCollector.Collect(source, Subject, optionIndex);
        }
        else
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {RepairCost} gold.");
        }
    }
}