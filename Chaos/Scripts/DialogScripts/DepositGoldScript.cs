using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
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
    public class DepositGoldScript : DialogScriptBase
    {

        public DepositGoldScript(Dialog subject) : base(subject)
        {
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (!Subject.MenuArgs.TryGet<int>(0, out var amount))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            if (!source.TryTakeGold(amount))
            {
                Subject.Reply(source, "You don't have enough gold.");
                return;
            }
            source.Bank.AddGold((uint)amount);
            Subject.NextDialogKey = Subject.Template.TemplateKey;
        }
    }
}
