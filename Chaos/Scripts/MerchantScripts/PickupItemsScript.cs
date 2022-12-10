using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.MerchantScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MerchantScripts
{
    public class PickupItemsScript : MerchantScriptBase
    {
        List<string> sayings = new List<string>() { "Oh, I could use one of these!", "Yoink!", "You missed the altar..", "King Bruce gives a good payment for {Item}!", "Daddy needs a new pair of {Item}!", "Glioca blessed be!" };
        public PickupItemsScript(Merchant subject) : base(subject)
        {
        }

        public override void Update(TimeSpan delta)
        {
            var now = DateTime.UtcNow;
            var timeSpan = TimeSpan.FromSeconds(3);
            foreach (var item in Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject).Where(x => now - x.Creation > timeSpan))
            {
                Subject.MapInstance.RemoveObject(item);
                var saying = sayings.PickRandom().Inject(item.Name);
                Subject.Say(saying);
            }
        }
    }
}
