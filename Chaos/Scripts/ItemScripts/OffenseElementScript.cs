using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ItemScripts
{
    public class OffenseElementScript : ConfigurableItemScriptBase
    {
        protected Element Element {  get; init; }

        public OffenseElementScript(Item subject) : base(subject)
        {
            
        }

        public override void OnEquipped(Aisling aisling)
        {
            aisling.StatSheet.SetOffenseElement(Element);
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
        }

        public override void OnUnEquipped(Aisling aisling)
        {
            aisling.StatSheet.SetOffenseElement(Element.None);
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
        }
    }
}
