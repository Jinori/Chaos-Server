using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MerchantScripts
{
    public class DeferredDialogScript : MerchantScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;

        public DeferredDialogScript(Merchant subject, IMerchantFactory merchantFactory) : base(subject)
        {
            this.MerchantFactory = merchantFactory;
        }

        public override void OnClicked(Aisling source)
        {
            var merchant = MerchantFactory.Create("terrorChest", source.MapInstance, new Point(source.X, source.Y));
            merchant.OnClicked(source);
        }
    }
}
