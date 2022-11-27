using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class WithdrawGold : DialogScriptBase
    {
        public WithdrawGold(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            var gold = source.Bank.Gold;
            Subject.Text = $"You currently have {gold} coins stashed. Would you like to withdraw more?";
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (!Subject.MenuArgs.TryGet<int>(0, out var amount))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            if (source.Bank.Gold < amount)
            {
                Subject.Reply(source, "You don't have enough gold.");
                return;
            }
            if (!source.TryGiveGold(amount))
            {
                Subject.Reply(source, "You have too much gold!");
                return;
            }
            source.Bank.RemoveGold((uint)amount);
            Subject.NextDialogKey = Subject.Template.TemplateKey;
        }
    }
}
