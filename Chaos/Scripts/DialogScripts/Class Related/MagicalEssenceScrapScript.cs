using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts.Class_Related
{
    public class MagicalEssenceScrapScript : DialogScriptBase
    {

        public Item? Ask { get; set; }
        public InputCollector InputCollector { get; }
        private readonly IItemFactory ItemFactory;


        public MagicalEssenceScrapScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory= itemFactory;
            var requestInputText = DialogString.From(() => $"How many {Ask!.DisplayName} would you like to turn into magical essence? You have {Ask!.Count}.");

            InputCollector = new InputCollectorBuilder()
                             .RequestTextInput(requestInputText)
                             .HandleInput(HandleInputText)
                             .Build();
        }

        private bool HandleInputText(Aisling source, Dialog dialog, int? option)
        {
            if (!Subject.MenuArgs.TryGet<int>(1, out var amount))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return false;
            }
            if (!source.Inventory.HasCount(Ask!.DisplayName, amount))
            {
                Subject.Reply(source, $"You do not have that many {Ask.DisplayName}.");
                return false;
            }
            source.Inventory.RemoveQuantity(Ask.Slot, amount, out var Items);
            foreach (var Item in Items!)
            {
                var item = ItemFactory.Create("magicalessence");
                item.Count = amount;
                source.TryGiveItems(item);
            }
            Subject.NextDialogKey = Subject.Template.TemplateKey;
            return true;
        }

        public override void OnDisplaying(Aisling source)
        {
            var inventory = source.Inventory.Where(x => x.Template.SellValue >= 500).ToList();
            var slot = inventory.Select(x => x.Slot).ToList();
            Subject.Slots = slot;
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (Ask == null)
            {
                if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
                var Item = source.Inventory[slot];
                if (Item == null)
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
                Ask = Item;
            }
            if (source.Inventory.CountOf(Ask.DisplayName) == 1)
            {
                source.Inventory.RemoveQuantity(Ask.Slot, 1, out var Items);
                foreach (var Item in Items!)
                {
                    var item = ItemFactory.Create("magicalessence");
                    item.Count = 1;
                    source.TryGiveItems(item);
                }
                Subject.NextDialogKey = Subject.Template.TemplateKey;
            }
            else
                InputCollector.Collect(source, Subject, optionIndex);
        }
    }
}
