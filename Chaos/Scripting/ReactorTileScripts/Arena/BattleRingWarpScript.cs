using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Arena
{
    public class BattleRingWarpScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : ReactorTileScriptBase(subject)
    {
        public override void OnWalkedOn(Creature source)
        {
            if (source is not Aisling aisling)
                return;
            
            var merchant = Subject.MapInstance.GetEntities<Merchant>().FirstOrDefault(x => x.Name == "Rlyeh");
            if (merchant == null) 
                return;
            
            var dialog = dialogFactory.Create("rlyeh_battlering", merchant);
            dialog.Display(aisling);
        }
    }
}