using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class WithdrawItemScript : DialogScriptBase
    {
        private readonly InputCollector InputCollector;
        private readonly IItemFactory ItemFactory;
        private readonly ILogger<BuyShopScript> Logger;
        private int? Amount;
        private Item? ShopItem;
        private int? TotalBuyCost;

        /// <inheritdoc />
        public WithdrawItemScript(Dialog subject, IItemFactory itemFactory, ILogger<BuyShopScript> logger)
            : base(subject)
        {
            Logger = logger;
            ItemFactory = itemFactory;
            var requestInputText = DialogString.From(() => $"How many {ShopItem!.DisplayName} would you like to withdraw?");

            InputCollector = new InputCollectorBuilder()
                             .RequestTextInput(requestInputText)
                             .HandleInput(HandleTextInput)
                             .Build();
        }

        public override void OnDisplaying(Aisling source)
        {
            if (!Subject.Items.Any())
            {
                Subject.Items.AddRange(source.Bank.Select(x => new Data.ItemDetails { Item = x, AmountOrPrice = x.Count }));
            }
        }

        private bool HandleTextInput(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (!Subject.MenuArgs.TryGet<int>(1, out var amount) || (amount <= 0))
            {
                dialog.Reply(aisling, DialogString.UnknownInput.Value);

                return false;
            }

            ShopItem!.Count = amount;
            Amount = amount;


            if (aisling.Bank.CountOf(ShopItem!.DisplayName) < Amount)
            {
                dialog.Reply(aisling, "What? I think I need to take better inventory. Its not here.");
                return false;
            }
            if (!aisling.CanCarry(ShopItem))
            {
                dialog.Reply(aisling, "You have no space! Please make room on yourself.");
                return false;
            }
            aisling.Bank.TryWithdraw(ShopItem.DisplayName, Amount.Value, out var Items);
            foreach (var itemS in Items!)
            {
                aisling.Inventory.TryAddToNextSlot(itemS);
            }
            Subject.NextDialogKey = Subject.Template.TemplateKey;
            return true;
        }

        /// <inheritdoc />
        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (ShopItem == null)
            {
                if (!Subject.MenuArgs.Any())
                    return;

                var itemName = Subject.MenuArgs.First();
                ShopItem = Subject.Items.Select(i => i.Item).FirstOrDefault(i => i.DisplayName.EqualsI(itemName));

                if (ShopItem == null)
                    return;
            }
            if (source.Bank.CountOf(ShopItem!.DisplayName) == 1)
            {
                source.Bank.TryWithdraw(ShopItem.DisplayName, 1, out var Items);
                foreach (var itemS in Items!)
                {
                    source.Inventory.TryAddToNextSlot(itemS);
                }
                Subject.NextDialogKey = Subject.Template.TemplateKey;
            }
            InputCollector.Collect(source, Subject, optionIndex);
        }
    }
}
