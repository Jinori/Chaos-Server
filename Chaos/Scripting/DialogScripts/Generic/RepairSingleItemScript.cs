using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic
{
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
                var damage = (float)item.CurrentDurability.Value / item.Template.MaxDurability.Value;
                var formula = item.Template.SellValue / 2.0 * (.8 * damage);
                RepairCost = (int)(RepairCost + formula);

                if (!source.TryTakeGold(RepairCost))
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage($"You do not have enough gold. You need {RepairCost} gold.");
                    return;
                }

                source.Inventory.Update(slot, item1 =>
                {
                    item1.CurrentDurability = item1.Template.MaxDurability;
                    item1.LastWarningLevel = 100;
                });

                source.SendOrangeBarMessage($"Your {item.DisplayName} has been repaired.");
                Subject.InjectTextParameters(item.DisplayName, RepairCost);
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
                var damage = (float)item.CurrentDurability.Value / item.Template.MaxDurability.Value;
                var formula = item.Template.SellValue / 2.0 * (.8 * damage);
                RepairCost = (int)(RepairCost + formula);

                Subject.InjectTextParameters(item.DisplayName, RepairCost);
            }
        }

        private void OnDisplayingInitial(Aisling source)
        {
            var itemsToRepair = source.Inventory.Where(x =>
                (x.Template.MaxDurability != null) && (x.CurrentDurability != null) &&
                (x.CurrentDurability.Value != x.Template.MaxDurability.Value)).ToList();

            if (itemsToRepair.Count == 0)
            {
                Subject.Reply(source, "All of your inventory items are already fully repaired!");
                return;
            }

            Subject.Slots = itemsToRepair.Select(x => x.Slot).ToList();
        }
    }
}