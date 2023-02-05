using Chaos.Common.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts
{
    public class GiftboxScript : ConfigurableItemScriptBase
    {
        public GiftboxScript(Item subject) : base(subject)
        {
        }

        protected int? GoldAmount { get; init; }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                if (GoldAmount.HasValue)
                {
                    //Give Gold Amount
                    source.TryGiveGold(GoldAmount.Value);
                    //Give Legend Mark
                    source.Legend.AddOrAccumulate(new LegendMark("Opened a " + Subject.DisplayName + "!", "giftbox1", MarkIcon.Yay, MarkColor.White, 1, Time.GameTime.Now));
                    //Remove box from Inventory
                    source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
                    //Display confirmation to user
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've received " + GoldAmount.Value + " from " + Subject.DisplayName +  "!");
                }
            }
        }
    }
}
