using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RepairItemScript : DialogScriptBase
{
    private int _repairCost;

    public RepairItemScript(Dialog subject)
        : base(subject)
    {
        
    }

    public override void OnDisplaying(Aisling source)
    {
        foreach (var item in source.Equipment)
        {
            if (item.Template.MaxDurability != null && item.CurrentDurability != null && item.CurrentDurability.Value != item.Template.MaxDurability.Value)
            {
                var damage = ((float)item.CurrentDurability.Value / item.Template.MaxDurability.Value);
                var formula = item.Template.SellValue / 2.0 * (.8 * damage);
                _repairCost = (((int)(_repairCost + formula)));
            }
        }
        
        if (_repairCost != 0) 
            Subject.Text = $"I can repair all of your items for {_repairCost}. Would you like to continue?";
        else
        {
            Subject.Text = "I cannot repair your items any further.";
            Subject.Type = MenuOrDialogType.Normal;
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!source.TryTakeGold(_repairCost))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You do not have enough. You need {_repairCost} gold.");
            return;
        }
        
        foreach (var repair in source.Equipment)
        {
            if (repair.CurrentDurability != repair.Template.MaxDurability)
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
            }
        }
        source.SendOrangeBarMessage($"Your items have been repaired.");
    }
}