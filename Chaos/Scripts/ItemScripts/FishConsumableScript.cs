using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ItemScripts
{
    public class FishConsumableScript : ItemScriptBase
    {
        private protected int percent { get; set; }

        public FishConsumableScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {

            int TNL = LevelUpFormulae.Default.GetNewTnl(source);

            switch (Subject.DisplayName)
            {
                case "Trout":
                    percent = Convert.ToInt32(0.006 * TNL);
                    break;
                case "Bass":
                    percent = Convert.ToInt32(0.007 * TNL);
                    break;
                case "Perch":
                    percent = Convert.ToInt32(0.008 * TNL);
                    break;
                case "Pike":
                    percent = Convert.ToInt32(0.009 * TNL);
                    break;
                case "Rock Fish":
                    percent = Convert.ToInt32(0.01 * TNL);
                    break;
                case "Lion Fish":
                    percent = Convert.ToInt32(0.02 * TNL);
                    break;
                case "Purple Whopper":
                    percent = Convert.ToInt32(0.03 * TNL);
                    break;
            }

            source.GiveExp(percent);
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You ate {Subject.DisplayName} and it gave you {percent} exp.");
            source.Legend.AddOrAccumulate(new Objects.Legend.LegendMark("Caught a fish and ate it", "fish", MarkIcon.Yay, MarkColor.White, 1, Time.GameTime.Now));
        }
    }
}
